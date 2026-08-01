using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using ETLVentas.DW.application.Interfaces.Services;
using ETLVentas.DW.application.Models.Dtos;
using ETLVentas.DW.application.Models.Dtos.Csv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ETLVentas.DW.persistencia.Extractors
{
    public class CsvExtractorService : IDataExtractorService
    {
        private readonly ILogger<CsvExtractorService> _logger;
        private readonly string _csvFolderPath;

        public string SourceName => "CSV";

        public CsvExtractorService(ILogger<CsvExtractorService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _csvFolderPath = configuration["EtlSettings:CsvFolderPath"] 
                ?? throw new ArgumentException("La ruta de los CSV no está configurada en appsettings.json");
        }

        public async Task<IEnumerable<VentaExtraidaDto>> ExtractAsync()
        {
            _logger.LogInformation("[CSV] Iniciando extracción desde: {Path}", _csvFolderPath);

            try
            {
                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    MissingFieldFound = null
                };

                var customers = ReadCsv<CustomerCsvDto>(Path.Combine(_csvFolderPath, "customers.csv"), csvConfig);
                var products = ReadCsv<ProductCsvDto>(Path.Combine(_csvFolderPath, "products.csv"), csvConfig);
                var orders = ReadCsv<OrderCsvDto>(Path.Combine(_csvFolderPath, "orders.csv"), csvConfig);
                var orderDetails = ReadCsv<OrderDetailCsvDto>(Path.Combine(_csvFolderPath, "order_details.csv"), csvConfig);

                _logger.LogInformation("[CSV] Leídos: {Cust} clientes, {Prod} productos, {Ord} órdenes, {Det} detalles",
                    customers.Count, products.Count, orders.Count, orderDetails.Count);

                var ventasPlanas = (from det in orderDetails
                                    join ord in orders on det.OrderID equals ord.OrderID
                                    join cust in customers on ord.CustomerID equals cust.CustomerID
                                    join prod in products on det.ProductID equals prod.ProductID
                                    select new VentaExtraidaDto
                                    {
                                        OrderID = ord.OrderID,
                                        CustomerID = cust.CustomerID,
                                        FirstName = cust.FirstName,
                                        LastName = cust.LastName,
                                        Email = cust.Email,
                                        CityName = cust.City,
                                        CountryName = cust.Country,
                                        ProductID = prod.ProductID,
                                        ProductName = prod.ProductName,
                                        CategoryName = prod.Category,
                                        UnitPrice = prod.Price,
                                        Quantity = det.Quantity,
                                        SaleDate = ord.OrderDate,
                                        SourceName = "CSV"
                                    }).ToList();

                _logger.LogInformation("[CSV] Extracción completada: {Total} registros desnormalizados", ventasPlanas.Count);
                return await Task.FromResult(ventasPlanas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CSV] Error durante la extracción");
                return Enumerable.Empty<VentaExtraidaDto>();
            }
        }

        private List<T> ReadCsv<T>(string filePath, CsvConfiguration config)
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);
            return csv.GetRecords<T>().ToList();
        }
    }
}
