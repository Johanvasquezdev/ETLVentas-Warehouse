using ETLVentas.DW.domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETLVentas.DW.persistencia.Configurations
{
    public class DimProductConfiguration : IEntityTypeConfiguration<DimProduct>
    {
        public void Configure(EntityTypeBuilder<DimProduct> builder)
        {
            builder.ToTable("DimProduct");
            builder.HasKey(e => e.ProductKey);
            builder.Property(e => e.ProductName).HasMaxLength(100).IsRequired();
            builder.Property(e => e.CategoryName).HasMaxLength(100).IsRequired();
            builder.Property(e => e.UnitPrice).HasColumnType("decimal(10,2)").IsRequired();
        }
    }
}
