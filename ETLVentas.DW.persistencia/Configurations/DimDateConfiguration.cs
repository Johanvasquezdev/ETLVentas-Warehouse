using ETLVentas.DW.domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETLVentas.DW.persistencia.Configurations
{
    public class DimDateConfiguration : IEntityTypeConfiguration<DimDate>
    {
        public void Configure(EntityTypeBuilder<DimDate> builder)
        {
            builder.ToTable("DimDate");
            builder.HasKey(e => e.DateKey);
            builder.Property(e => e.DateKey).ValueGeneratedNever(); // No es identity
            builder.Property(e => e.DayName).HasMaxLength(20).IsRequired();
            builder.Property(e => e.MonthName).HasMaxLength(20).IsRequired();
            builder.Property(e => e.FullDate).HasColumnType("date").IsRequired();
        }
    }
}
