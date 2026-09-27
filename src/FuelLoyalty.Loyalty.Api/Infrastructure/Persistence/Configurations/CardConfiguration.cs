using FuelLoyalty.Loyalty.Api.Domain.Cards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FuelLoyalty.Loyalty.Api.Infrastructure.Persistence.Configurations
{
    public sealed class CardConfiguration : IEntityTypeConfiguration<Card>
    {
        public void Configure(EntityTypeBuilder<Card> builder)
        {
            builder.ToTable("Cards");

            builder.HasKey(c => c.Id);
            builder.HasIndex(c => c.CardNumber).IsUnique();

            builder.Property(c => c.CardNumber).HasMaxLength(20).IsRequired();
            builder.Property(c => c.HolderName).HasMaxLength(100).IsRequired();
            builder.Property(c => c.AllowedFuelType).HasConversion<string>().HasMaxLength(20);
            builder.Property(c => c.LimitType).HasConversion<string>().HasMaxLength(10);
            builder.Property(c => c.Balance).HasPrecision(18, 2);
            builder.Property(c => c.ReservedBalance).HasPrecision(18, 2);

            // PostgreSQL'in her satırda tuttuğu xmin kolonu sürüm numarası olarak kullanılır.
            builder.Property(c => c.Version).IsRowVersion();

            // Hesaplanan alan, veritabanında kolonu yok.
            builder.Ignore(c => c.AvailableBalance);
        }
    }
}
