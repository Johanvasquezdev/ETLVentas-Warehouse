using System;
using System.Threading.Tasks;
using ETLVentas.DW.application.Interfaces.Services;
using ETLVentas.DW.application.Models.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ETLVentas.DW.persistencia.Services
{
    public class DbCleanupService : IDbCleanupService
    {
        private readonly DWContext _context;
        private readonly ILogger<DbCleanupService> _logger;

        public DbCleanupService(DWContext context, ILogger<DbCleanupService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<OperationResult> CleanDimensionsAsync()
        {
            _logger.LogInformation("=== LIMPIEZA DE DIMENSIONES ===");

            try
            {
                await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE FactSales");
                _logger.LogInformation("[Cleanup] FactSales vaciada (dependencia FK).");

                await _context.Database.ExecuteSqlRawAsync("DELETE FROM DimCustomer");
                _logger.LogInformation("[Cleanup] DimCustomer vaciada.");

                await _context.Database.ExecuteSqlRawAsync("DELETE FROM DimProduct");
                _logger.LogInformation("[Cleanup] DimProduct vaciada.");

                await _context.Database.ExecuteSqlRawAsync("DELETE FROM DimDate");
                _logger.LogInformation("[Cleanup] DimDate vaciada.");

                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('DimCustomer', RESEED, 0)");
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('DimProduct', RESEED, 0)");
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('FactSales', RESEED, 0)");

                _logger.LogInformation("[Cleanup] Contadores de identidad reseteados.");

                return OperationResult.Ok("Dimensiones limpiadas correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Cleanup] Error limpiando dimensiones.");
                return OperationResult.Fail("Error en la limpieza de dimensiones.", ex);
            }
        }

        public async Task<OperationResult> CleanFactsAsync()
        {
            _logger.LogInformation("=== LIMPIEZA DE TABLA DE HECHOS ===");

            try
            {
                await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE FactSales");
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('FactSales', RESEED, 0)");

                _logger.LogInformation("[Cleanup] FactSales vaciada y reseteada.");
                return OperationResult.Ok("FactSales limpiada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Cleanup] Error limpiando FactSales.");
                return OperationResult.Fail("Error en la limpieza de FactSales.", ex);
            }
        }
    }
}
