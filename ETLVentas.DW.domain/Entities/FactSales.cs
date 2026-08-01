using System;

namespace ETLVentas.DW.domain.Entities
{
    public class FactSales
    {
        public long SalesKey { get; set; }
        public int OrderID { get; set; }
        
        public int DateKey { get; set; }
        public int CustomerKey { get; set; }
        public int ProductKey { get; set; }
        public int SourceKey { get; set; }
        
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalSale { get; set; }

        public virtual DimDate? Date { get; set; }
        public virtual DimCustomer? Customer { get; set; }
        public virtual DimProduct? Product { get; set; }
        public virtual DimSource? Source { get; set; }
    }
}
