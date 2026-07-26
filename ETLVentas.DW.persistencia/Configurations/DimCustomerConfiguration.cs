using ETLVentas.DW.domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETLVentas.DW.persistencia.Configurations
{
    public class DimCustomerConfiguration : IEntityTypeConfiguration<DimCustomer>
    {
        public void Configure(EntityTypeBuilder<DimCustomer> builder)
        {
            builder.ToTable("DimCustomer");
            builder.HasKey(e => e.CustomerKey);
            builder.Property(e => e.FirstName).HasMaxLength(50);
            builder.Property(e => e.LastName).HasMaxLength(50);
            builder.Property(e => e.Email).HasMaxLength(100);
            builder.Property(e => e.CityName).HasMaxLength(100).IsRequired();
            builder.Property(e => e.CountryName).HasMaxLength(100).IsRequired();
        }
    }
}
