using ETLVentas.DW.application.Interfaces.Services;
using ETLVentas.DW.persistencia;
using ETLVentas.DW.persistencia.Extractors;
using ETLVentas.DW.persistencia.Services;
using ETLVentas.DW.workerLoad;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) =>
{
    // Entity Framework Core -> DW
    services.AddDbContext<DWContext>(options =>
    {
        options.UseSqlServer(hostContext.Configuration.GetConnectionString("DWConnection"));
    });

    // Repositorio Genérico (DIP)
    services.AddScoped(typeof(ETLVentas.DW.application.Interfaces.Repositories.IBaseRepository<>),
                       typeof(ETLVentas.DW.persistencia.Repositories.BaseRepository<>));

    // Extractores (SOC: cada uno extrae de una fuente distinta)
    services.AddScoped<IDataExtractorService, CsvExtractorService>();
    services.AddScoped<IDataExtractorService, ApiExtractorService>();
    services.AddScoped<IDataExtractorService, DbExtractorService>();

    // HttpClient para el ApiExtractor
    services.AddHttpClient<ApiExtractorService>();

    // Servicios de Limpieza y Orquestación
    services.AddScoped<IDbCleanupService, DbCleanupService>();
    services.AddScoped<IEtlOrchestratorService, EtlOrchestratorService>();

    // Worker (BackgroundService)
    services.AddHostedService<Worker>();
});

using IHost host = builder.Build();
host.Run();
