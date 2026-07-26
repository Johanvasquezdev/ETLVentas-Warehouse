using ETLVentas.DW.persistencia;
using ETLVentas.DW.workerLoad;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) =>
{
    // Agregar la conexión a Entity Framework Core
    services.AddDbContext<DWContext>(options =>
    {
        options.UseSqlServer(hostContext.Configuration.GetConnectionString("DWConnection"));
    });

    services.AddHostedService<Worker>();
});

using IHost host = builder.Build();
host.Run();
