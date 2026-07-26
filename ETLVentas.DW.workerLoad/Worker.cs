using ETLVentas.DW.application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ETLVentas.DW.workerLoad
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostApplicationLifetime _lifetime;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider, IHostApplicationLifetime lifetime)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _lifetime = lifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("╔══════════════════════════════════════════════════════════╗");
            _logger.LogInformation("║       ETL VENTAS - DATA WAREHOUSE LOADER                ║");
            _logger.LogInformation("║       Inicio: {Fecha}                    ║", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            _logger.LogInformation("╚══════════════════════════════════════════════════════════╝");

            try
            {
                // Crear un Scope para resolver los servicios Scoped (como DbContext y EtlOrchestratorService)
                using (var scope = _serviceProvider.CreateScope())
                {
                    var etlOrchestrator = scope.ServiceProvider.GetRequiredService<IEtlOrchestratorService>();

                    // ─── TAREA 1: CARGAR DIMENSIONES ───
                    _logger.LogInformation("");
                    var dimResult = await etlOrchestrator.CargarDimensionesAsync();

                    if (!dimResult.Success)
                    {
                        _logger.LogError("FALLO en la carga de dimensiones: {Msg}", dimResult.Message);
                        _lifetime.StopApplication();
                        return;
                    }

                    _logger.LogInformation("[TAREA 1] Resultado: {Msg}", dimResult.Message);

                    // ─── TAREA 2: CARGAR HECHOS ───
                    _logger.LogInformation("");
                    var factResult = await etlOrchestrator.CargarFactSalesAsync();

                    if (!factResult.Success)
                    {
                        _logger.LogError("FALLO en la carga de hechos: {Msg}", factResult.Message);
                        _lifetime.StopApplication();
                        return;
                    }

                    _logger.LogInformation("[TAREA 2] Resultado: {Msg}", factResult.Message);
                }

                // ─── RESUMEN FINAL ───
                _logger.LogInformation("");
                _logger.LogInformation("╔══════════════════════════════════════════════════════════╗");
                _logger.LogInformation("║       PROCESO ETL FINALIZADO EXITOSAMENTE               ║");
                _logger.LogInformation("║       Fin: {Fecha}                       ║", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                _logger.LogInformation("╚══════════════════════════════════════════════════════════╝");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fatal no controlado en el Worker.");
            }
            finally
            {
                _lifetime.StopApplication();
            }
        }
    }
}
