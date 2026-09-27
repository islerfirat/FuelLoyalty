using FuelLoyalty.Sales.Api.Domain.Sales;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace FuelLoyalty.Sales.Api.Infrastructure.Persistence
{
    public sealed class SalesDbContext(DbContextOptions<SalesDbContext> options) : DbContext(options)
    {
        public DbSet<Sale> Sales => Set<Sale>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SalesDbContext).Assembly);

            // MassTransit Outbox tabloları (giden mesajlar burada bekler)
            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
        }
    }
}
