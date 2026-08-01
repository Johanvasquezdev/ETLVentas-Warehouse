using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ETLVentas.DW.application.Interfaces.Services;
using ETLVentas.DW.application.Models.Dtos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ETLVentas.DW.persistencia.Extractors
{
    public class ApiExtractorService : IDataExtractorService
    {
        private readonly ILogger<ApiExtractorService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public string SourceName => "API";

        public ApiExtractorService(ILogger<ApiExtractorService> logger, HttpClient httpClient, IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = httpClient;
            _apiUrl = configuration["EtlSettings:ApiUrl"] 
                ?? throw new ArgumentException("La URL de la API no está configurada en appsettings.json");
        }

        public async Task<IEnumerable<VentaExtraidaDto>> ExtractAsync()
        {
            _logger.LogInformation("[API] Iniciando extracción desde: {Url}", _apiUrl);

            try
            {
                var ventasApi = await _httpClient.GetFromJsonAsync<List<VentaExtraidaDto>>(_apiUrl);

                if (ventasApi == null || !ventasApi.Any())
                {
                    _logger.LogWarning("[API] La API no devolvió registros.");
                    return Enumerable.Empty<VentaExtraidaDto>();
                }

                foreach (var venta in ventasApi)
                {
                    venta.SourceName = "API";
                }

                _logger.LogInformation("[API] Extracción completada: {Total} registros obtenidos", ventasApi.Count);
                return ventasApi;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[API] Error durante la extracción desde la API");
                return Enumerable.Empty<VentaExtraidaDto>();
            }
        }
    }
}
