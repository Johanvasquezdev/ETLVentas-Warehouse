using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ETLVentas.DW.application.Interfaces.Services;
using ETLVentas.DW.application.Models.Dtos;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ETLVentas.DW.persistencia.Extractors
{
    public class DbExtractorService : IDataExtractorService
    {
        private readonly ILogger<DbExtractorService> _logger;
        private readonly string _connectionString;

        public string SourceName => "External Database";

        public DbExtractorService(ILogger<DbExtractorService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("ExternalDbConnection") 
                ?? throw new ArgumentException("La cadena de conexión ExternalDbConnection no está configurada");
        }

        public async Task<IEnumerable<VentaExtraidaDto>> ExtractAsync()
        {
            _logger.LogInformation("[BD Externa] Iniciando extracción desde AnalisisVentas...");

            var ventas = new List<VentaExtraidaDto>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    SELECT 
                        o.OrderID,
                        c.CustomerID,
                        c.FirstName,
                        c.LastName,
                        c.Email,
                        ci.CityName,
                        co.CountryName,
                        p.ProductID,
                        p.ProductName,
                        cat.CategoryName,
                        p.UnitPrice,
                        od.Quantity,
                        o.OrderDate AS SaleDate
                    FROM Orders o
                    INNER JOIN Customers c ON o.CustomerID = c.CustomerID
                    INNER JOIN Cities ci ON c.CityID = ci.CityID
                    INNER JOIN Countries co ON ci.CountryID = co.CountryID
                    INNER JOIN OrderDetails od ON o.OrderID = od.OrderID
                    INNER JOIN Products p ON od.ProductID = p.ProductID
                    INNER JOIN Categories cat ON p.CategoryID = cat.CategoryID";

                using var command = new SqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    ventas.Add(new VentaExtraidaDto
                    {
                        OrderID = reader.GetInt32(0),
                        CustomerID = reader.GetInt32(1),
                        FirstName = reader.GetString(2),
                        LastName = reader.GetString(3),
                        Email = reader.GetString(4),
                        CityName = reader.GetString(5),
                        CountryName = reader.GetString(6),
                        ProductID = reader.GetInt32(7),
                        ProductName = reader.GetString(8),
                        CategoryName = reader.GetString(9),
                        UnitPrice = reader.GetDecimal(10),
                        Quantity = reader.GetInt32(11),
                        SaleDate = reader.GetDateTime(12),
                        SourceName = "External Database"
                    });
                }

                _logger.LogInformation("[BD Externa] Extracción completada: {Total} registros obtenidos", ventas.Count);
                return ventas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[BD Externa] Error durante la extracción");
                return Enumerable.Empty<VentaExtraidaDto>();
            }
        }
    }
}
