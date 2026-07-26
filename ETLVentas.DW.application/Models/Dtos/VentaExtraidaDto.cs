using System;

namespace ETLVentas.DW.application.Models.Dtos
{
    /// <summary>
    /// DTO unificado de salida para todos los extractores.
    /// Representa una venta desnormalizada (plana) lista para ser transformada.
    /// </summary>
    public class VentaExtraidaDto
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CityName { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public DateTime SaleDate { get; set; }
        public string SourceName { get; set; } = string.Empty; // CSV, API, External Database
    }
}
