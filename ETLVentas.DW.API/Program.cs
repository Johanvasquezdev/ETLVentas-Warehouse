using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ETLVentas.DW.application.Models.Dtos;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// ENDPOINT DE VENTAS (MOCK PARA ETL)
app.MapGet("/api/ventas", () =>
{
    var ventasMock = new List<VentaExtraidaDto>
    {
        new VentaExtraidaDto
        {
            OrderID = 90001,
            CustomerID = 777,
            FirstName = "Juan",
            LastName = "Perez",
            Email = "juan@api.com",
            CityName = "Santo Domingo",
            CountryName = "República Dominicana",
            ProductID = 888,
            ProductName = "Laptop Pro 15",
            CategoryName = "Electrónica",
            UnitPrice = 1500.00m,
            Quantity = 1,
            SaleDate = new DateTime(2026, 07, 26),
            SourceName = "API"
        },
        new VentaExtraidaDto
        {
            OrderID = 90002,
            CustomerID = 778,
            FirstName = "Maria",
            LastName = "Gomez",
            Email = "maria@api.com",
            CityName = "Santiago",
            CountryName = "República Dominicana",
            ProductID = 889,
            ProductName = "Monitor 4K",
            CategoryName = "Electrónica",
            UnitPrice = 300.00m,
            Quantity = 2,
            SaleDate = new DateTime(2026, 07, 26),
            SourceName = "API"
        }
    };

    return ventasMock;
})
.WithName("GetVentas");

app.Run();
