using ETLVentas.DW.domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETLVentas.DW.persistencia.Configurations
{
    public class DimSourceConfiguration : IEntityTypeConfiguration<DimSource>
    {
        public void Configure(EntityTypeBuilder<DimSource> builder)
        {
            builder.ToTable("DimSource");
            builder.HasKey(e => e.SourceKey);
            builder.Property(e => e.SourceName).HasMaxLength(50).IsRequired();
            builder.Property(e => e.Description).HasMaxLength(200);
        }
    }
}
