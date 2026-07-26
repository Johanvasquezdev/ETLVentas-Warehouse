using ETLVentas.DW.application.Interfaces.Services;
using ETLVentas.DW.persistencia;
using ETLVentas.DW.persistencia.Extractors;
using ETLVentas.DW.persistencia.Services;
using ETLVentas.DW.workerLoad;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using System.IO;

var builder = Host.CreateDefaultBuilder(args);

// Configuracion de logging ultra-limpio para la presentacion
builder.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole(options => options.FormatterName = "clean");
    logging.AddConsoleFormatter<CleanConsoleFormatter, ConsoleFormatterOptions>();
});

builder.ConfigureServices((hostContext, services) =>
{
    services.AddDbContext<DWContext>(options =>
    {
        options.UseSqlServer(hostContext.Configuration.GetConnectionString("DWConnection"));
    });

    services.AddScoped(typeof(ETLVentas.DW.application.Interfaces.Repositories.IBaseRepository<>),
                       typeof(ETLVentas.DW.persistencia.Repositories.BaseRepository<>));

    services.AddScoped<IDataExtractorService, CsvExtractorService>();
    services.AddScoped<IDataExtractorService, ApiExtractorService>();
    services.AddScoped<IDataExtractorService, DbExtractorService>();

    services.AddHttpClient<ApiExtractorService>();

    services.AddScoped<IDbCleanupService, DbCleanupService>();
    services.AddScoped<IEtlOrchestratorService, EtlOrchestratorService>();

    services.AddHostedService<Worker>();
});

using IHost host = builder.Build();
host.Run();

public sealed class CleanConsoleFormatter : ConsoleFormatter
{
    public CleanConsoleFormatter() : base("clean") { }
    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
    {
        string? message = logEntry.Formatter(logEntry.State, logEntry.Exception);
        if (!string.IsNullOrEmpty(message))
        {
            textWriter.WriteLine(message);
        }
    }
}
