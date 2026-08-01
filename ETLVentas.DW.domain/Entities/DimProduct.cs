using System;

namespace ETLVentas.DW.domain.Entities
{
    public class DimProduct
    {
        public int ProductKey { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
    }
}
