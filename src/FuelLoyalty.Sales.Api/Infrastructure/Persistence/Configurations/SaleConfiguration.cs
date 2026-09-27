using FuelLoyalty.Sales.Api.Domain.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FuelLoyalty.Sales.Api.Infrastructure.Persistence.Configurations
{
    public sealed class SaleConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> builder)
        {
            builder.ToTable("Sales");

            builder.HasKey(s => s.Id);

            // Bir provizyonla yalnızca bir satış yapılabilir.
            builder.HasIndex(s => s.AuthorizationId).IsUnique();

            // Satış raporu tarihe göre filtreler ve sıralar.
            builder.HasIndex(s => s.SoldAt);

            builder.Property(s => s.StationCode).HasMaxLength(20).IsRequired();
            builder.Property(s => s.CardNumber).HasMaxLength(20).IsRequired();
            builder.Property(s => s.FuelType).HasConversion<string>().HasMaxLength(20);
            builder.Property(s => s.Liters).HasPrecision(18, 2);
            builder.Property(s => s.Amount).HasPrecision(18, 2);
        }
    }
}
