using ETLVentas.DW.domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ETLVentas.DW.persistencia
{
    public class DWContext : DbContext
    {
        public DWContext(DbContextOptions<DWContext> options) : base(options)
        {
        }

        public DbSet<DimCustomer> DimCustomers { get; set; }
        public DbSet<DimProduct> DimProducts { get; set; }
        public DbSet<DimDate> DimDates { get; set; }
        public DbSet<DimSource> DimSources { get; set; }
        public DbSet<FactSales> FactSales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
