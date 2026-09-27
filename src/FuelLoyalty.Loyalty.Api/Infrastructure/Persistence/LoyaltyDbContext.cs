using FuelLoyalty.Loyalty.Api.Domain.Authorizations;
using FuelLoyalty.Loyalty.Api.Domain.Cards;
using Microsoft.EntityFrameworkCore;

namespace FuelLoyalty.Loyalty.Api.Infrastructure.Persistence
{
    public sealed class LoyaltyDbContext(DbContextOptions<LoyaltyDbContext> options) : DbContext(options)
    {
        public DbSet<Card> Cards => Set<Card>();

        public DbSet<CardAuthorization> Authorizations => Set<CardAuthorization>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurations klasöründeki tüm tablo ayarlarını otomatik uygular.
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(LoyaltyDbContext).Assembly);
        }
    }
}
