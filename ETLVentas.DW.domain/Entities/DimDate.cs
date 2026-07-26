using System;

namespace ETLVentas.DW.domain.Entities
{
    public class DimDate
    {
        public int DateKey { get; set; }
        public DateTime FullDate { get; set; }
        public int DayNumber { get; set; }
        public string DayName { get; set; } = string.Empty;
        public int WeekNumber { get; set; }
        public int MonthNumber { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public int QuarterNumber { get; set; }
        public int YearNumber { get; set; }
        public bool IsWeekend { get; set; }
    }
}
