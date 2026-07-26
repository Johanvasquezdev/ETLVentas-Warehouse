using ETLVentas.DW.domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETLVentas.DW.persistencia.Configurations
{
    public class FactSalesConfiguration : IEntityTypeConfiguration<FactSales>
    {
        public void Configure(EntityTypeBuilder<FactSales> builder)
        {
            builder.ToTable("FactSales");
            builder.HasKey(e => e.SalesKey);
            builder.Property(e => e.UnitPrice).HasColumnType("decimal(10,2)").IsRequired();
            builder.Property(e => e.TotalSale).HasColumnType("decimal(15,2)").IsRequired();

            // Relaciones (Foreign Keys)
            builder.HasOne(e => e.Customer).WithMany().HasForeignKey(e => e.CustomerKey);
            builder.HasOne(e => e.Product).WithMany().HasForeignKey(e => e.ProductKey);
            builder.HasOne(e => e.Date).WithMany().HasForeignKey(e => e.DateKey);
            builder.HasOne(e => e.Source).WithMany().HasForeignKey(e => e.SourceKey);
        }
    }
}
