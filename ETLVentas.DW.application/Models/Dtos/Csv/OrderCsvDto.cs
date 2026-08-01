using System;

namespace ETLVentas.DW.application.Models.Dtos.Csv
{
    public class OrderCsvDto
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
