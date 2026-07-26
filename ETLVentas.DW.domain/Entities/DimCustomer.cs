using System;

namespace ETLVentas.DW.domain.Entities
{
    public class DimCustomer
    {
        public int CustomerKey { get; set; }
        public int CustomerID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string CityName { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
    }
}
