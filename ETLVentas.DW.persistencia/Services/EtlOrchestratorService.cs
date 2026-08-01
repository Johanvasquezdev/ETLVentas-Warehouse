using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ETLVentas.DW.application.Interfaces.Services;
using ETLVentas.DW.application.Models.Dtos;
using ETLVentas.DW.application.Models.Results;
using ETLVentas.DW.domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ETLVentas.DW.persistencia.Services
{
    public class EtlOrchestratorService : IEtlOrchestratorService
    {
        private readonly DWContext _context;
        private readonly IEnumerable<IDataExtractorService> _extractors;
        private readonly IDbCleanupService _cleanupService;
        private readonly ILogger<EtlOrchestratorService> _logger;

        private List<VentaExtraidaDto> _datosExtraidos = new();

        public EtlOrchestratorService(
            DWContext context,
            IEnumerable<IDataExtractorService> extractors,
            IDbCleanupService cleanupService,
            ILogger<EtlOrchestratorService> logger)
        {
            _context = context;
            _extractors = extractors;
            _cleanupService = cleanupService;
            _logger = logger;
        }

        public async Task<OperationResult> CargarDimensionesAsync()
        {
            _logger.LogInformation("╔══════════════════════════════════════════════════╗");
            _logger.LogInformation("║   INICIO: CARGA DE DIMENSIONES DEL DW           ║");
            _logger.LogInformation("╚══════════════════════════════════════════════════╝");

            try
            {
                _logger.LogInformation("───── PASO 1: LIMPIEZA ─────");
                var cleanResult = await _cleanupService.CleanDimensionsAsync();
                if (!cleanResult.Success) return cleanResult;

                _logger.LogInformation("───── PASO 2: EXTRACCIÓN ─────");
                _datosExtraidos = await ExtraerDatosAsync();
                if (!_datosExtraidos.Any())
                    return OperationResult.Fail("No se obtuvieron datos de ninguna fuente.");

                _logger.LogInformation("[Extracción] Total combinado: {Total} registros de {Fuentes} fuentes.",
                    _datosExtraidos.Count, _extractors.Count());

                _logger.LogInformation("───── PASO 3: TRANSFORMACIÓN Y CARGA ─────");

                var custResult = await CargarDimCustomerAsync(_datosExtraidos);
                _logger.LogInformation("[DimCustomer] {Status}: {Msg}", custResult.Success ? "OK" : "ERROR", custResult.Message);

                var prodResult = await CargarDimProductAsync(_datosExtraidos);
                _logger.LogInformation("[DimProduct] {Status}: {Msg}", prodResult.Success ? "OK" : "ERROR", prodResult.Message);

                var dateResult = await CargarDimDateAsync(_datosExtraidos);
                _logger.LogInformation("[DimDate] {Status}: {Msg}", dateResult.Success ? "OK" : "ERROR", dateResult.Message);

                _logger.LogInformation("╔══════════════════════════════════════════════════╗");
                _logger.LogInformation("║   FIN: CARGA DE DIMENSIONES COMPLETADA           ║");
                _logger.LogInformation("╚══════════════════════════════════════════════════╝");

                return OperationResult.Ok("Todas las dimensiones fueron cargadas exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fatal durante la carga de dimensiones.");
                return OperationResult.Fail("Error fatal en CargarDimensionesAsync.", ex);
            }
        }

        public async Task<OperationResult> CargarFactSalesAsync()
        {
            _logger.LogInformation("╔══════════════════════════════════════════════════╗");
            _logger.LogInformation("║   INICIO: CARGA DE TABLA DE HECHOS (FactSales)   ║");
            _logger.LogInformation("╚══════════════════════════════════════════════════╝");

            try
            {
                _logger.LogInformation("───── PASO 1: LIMPIEZA DE FACTS ─────");
                var cleanResult = await _cleanupService.CleanFactsAsync();
                if (!cleanResult.Success) return cleanResult;

                if (!_datosExtraidos.Any())
                {
                    _logger.LogInformation("───── PASO 2: RE-EXTRACCIÓN ─────");
                    _datosExtraidos = await ExtraerDatosAsync();
                }

                _logger.LogInformation("───── PASO 3: MAPEO DE CLAVES SURROGADAS ─────");

                var customerMap = await _context.DimCustomers
                    .AsNoTracking()
                    .ToDictionaryAsync(c => c.CustomerID, c => c.CustomerKey);

                var productMap = await _context.DimProducts
                    .AsNoTracking()
                    .ToDictionaryAsync(p => p.ProductID, p => p.ProductKey);

                var dateMap = await _context.DimDates
                    .AsNoTracking()
                    .ToDictionaryAsync(d => d.FullDate, d => d.DateKey);

                var sourceMap = await _context.DimSources
                    .AsNoTracking()
                    .ToDictionaryAsync(s => s.SourceName, s => s.SourceKey);

                _logger.LogInformation("[Mapeo] Claves cargadas - Clientes: {C}, Productos: {P}, Fechas: {D}, Fuentes: {S}",
                    customerMap.Count, productMap.Count, dateMap.Count, sourceMap.Count);

                _logger.LogInformation("───── PASO 4: CARGA DE HECHOS ─────");

                var factsList = new List<FactSales>();
                int omitidos = 0;

                foreach (var venta in _datosExtraidos)
                {
                    if (!customerMap.TryGetValue(venta.CustomerID, out int customerKey) ||
                        !productMap.TryGetValue(venta.ProductID, out int productKey) ||
                        !dateMap.TryGetValue(venta.SaleDate.Date, out int dateKey) ||
                        !sourceMap.TryGetValue(venta.SourceName, out int sourceKey))
                    {
                        omitidos++;
                        continue;
                    }

                    factsList.Add(new FactSales
                    {
                        OrderID = venta.OrderID,
                        DateKey = dateKey,
                        CustomerKey = customerKey,
                        ProductKey = productKey,
                        SourceKey = sourceKey,
                        Quantity = venta.Quantity,
                        UnitPrice = venta.UnitPrice,
                        TotalSale = venta.UnitPrice * venta.Quantity
                    });
                }

                int totalInsertados = 0;
                foreach (var batch in factsList.Chunk(5000))
                {
                    await _context.FactSales.AddRangeAsync(batch);
                    await _context.SaveChangesAsync();
                    _context.ChangeTracker.Clear();
                    totalInsertados += batch.Length;
                    _logger.LogInformation("[FactSales] Bloque insertado: {Count}/{Total}", totalInsertados, factsList.Count);
                }

                _logger.LogInformation("╔══════════════════════════════════════════════════╗");
                _logger.LogInformation("║   RESULTADO FINAL - TABLA DE HECHOS              ║");
                _logger.LogInformation("║   Insertados: {Ins,-10} | Omitidos: {Omit,-10}   ║", totalInsertados, omitidos);
                _logger.LogInformation("╚══════════════════════════════════════════════════╝");

                return OperationResult.Ok($"FactSales cargada: {totalInsertados} registros insertados, {omitidos} omitidos.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fatal durante la carga de FactSales.");
                return OperationResult.Fail("Error fatal en CargarFactSalesAsync.", ex);
            }
        }


        private async Task<List<VentaExtraidaDto>> ExtraerDatosAsync()
        {
            var todosLosDatos = new List<VentaExtraidaDto>();

            foreach (var extractor in _extractors)
            {
                try
                {
                    _logger.LogInformation("[Extractor] Ejecutando: {Source}...", extractor.SourceName);
                    var datos = await extractor.ExtractAsync();
                    todosLosDatos.AddRange(datos);
                    _logger.LogInformation("[Extractor] {Source}: {Count} registros extraídos.", extractor.SourceName, datos.Count());
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "[Extractor] Fallo en {Source}, continuando con las demás fuentes.", extractor.SourceName);
                }
            }

            return todosLosDatos;
        }

        private async Task<OperationResult> CargarDimCustomerAsync(List<VentaExtraidaDto> datos)
        {
            try
            {
                var customersUnicos = datos
                    .GroupBy(v => v.CustomerID)
                    .Select(g => g.First())
                    .Select(v => new DimCustomer
                    {
                        CustomerID = v.CustomerID,
                        FirstName = v.FirstName.Trim(),
                        LastName = v.LastName.Trim(),
                        Email = v.Email.Trim(),
                        CityName = v.CityName.Trim(),
                        CountryName = v.CountryName.Trim()
                    }).ToList();

                await _context.DimCustomers.AddRangeAsync(customersUnicos);
                await _context.SaveChangesAsync();
                _context.ChangeTracker.Clear();

                return OperationResult.Ok($"{customersUnicos.Count} clientes únicos cargados.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Error cargando DimCustomer.", ex);
            }
        }

        private async Task<OperationResult> CargarDimProductAsync(List<VentaExtraidaDto> datos)
        {
            try
            {
                var productosUnicos = datos
                    .GroupBy(v => v.ProductID)
                    .Select(g => g.First())
                    .Select(v => new DimProduct
                    {
                        ProductID = v.ProductID,
                        ProductName = v.ProductName.Trim(),
                        CategoryName = v.CategoryName.Trim(),
                        UnitPrice = v.UnitPrice
                    }).ToList();

                await _context.DimProducts.AddRangeAsync(productosUnicos);
                await _context.SaveChangesAsync();
                _context.ChangeTracker.Clear();

                return OperationResult.Ok($"{productosUnicos.Count} productos únicos cargados.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Error cargando DimProduct.", ex);
            }
        }

        private async Task<OperationResult> CargarDimDateAsync(List<VentaExtraidaDto> datos)
        {
            try
            {
                var fechasUnicas = datos
                    .Select(v => v.SaleDate.Date)
                    .Distinct()
                    .OrderBy(d => d)
                    .Select(fecha => new DimDate
                    {
                        DateKey = int.Parse(fecha.ToString("yyyyMMdd")),
                        FullDate = fecha,
                        DayNumber = fecha.Day,
                        DayName = fecha.ToString("dddd", CultureInfo.InvariantCulture),
                        WeekNumber = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                            fecha, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday),
                        MonthNumber = fecha.Month,
                        MonthName = fecha.ToString("MMMM", CultureInfo.InvariantCulture),
                        QuarterNumber = (fecha.Month - 1) / 3 + 1,
                        YearNumber = fecha.Year,
                        IsWeekend = fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday
                    }).ToList();

                await _context.DimDates.AddRangeAsync(fechasUnicas);
                await _context.SaveChangesAsync();
                _context.ChangeTracker.Clear();

                return OperationResult.Ok($"{fechasUnicas.Count} fechas únicas cargadas.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Error cargando DimDate.", ex);
            }
        }
    }
}
