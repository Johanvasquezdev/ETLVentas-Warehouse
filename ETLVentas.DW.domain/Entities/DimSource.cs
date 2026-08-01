using System;

namespace ETLVentas.DW.domain.Entities
{
    public class DimSource
    {
        public int SourceKey { get; set; }
        public string SourceName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
