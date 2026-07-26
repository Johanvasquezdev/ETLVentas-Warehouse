namespace ETLVentas.DW.application.Models.Dtos.Csv
{
    public class OrderDetailCsvDto
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
