using FuelLoyalty.Loyalty.Api.Domain.Authorizations;
using FuelLoyalty.Loyalty.Api.Domain.Cards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FuelLoyalty.Loyalty.Api.Infrastructure.Persistence.Configurations
{
    public sealed class CardAuthorizationConfiguration : IEntityTypeConfiguration<CardAuthorization>
    {
        public void Configure(EntityTypeBuilder<CardAuthorization> builder)
        {
            builder.ToTable("Authorizations");

            builder.HasKey(a => a.Id);

            // Provizyon mutlaka var olan bir karta ait olmalı; kartı olan provizyon varken kart silinemez.
            builder.HasOne<Card>()
                .WithMany()
                .HasForeignKey(a => a.CardId)
                .OnDelete(DeleteBehavior.Restrict);

            // Süresi dolan provizyon sorgusu için
            builder.HasIndex(a => new { a.Status, a.ExpiresAt });

            builder.Property(a => a.CardNumber).HasMaxLength(20).IsRequired();
            builder.Property(a => a.StationCode).HasMaxLength(20).IsRequired();
            builder.Property(a => a.FuelType).HasConversion<string>().HasMaxLength(20);
            builder.Property(a => a.LimitType).HasConversion<string>().HasMaxLength(10);
            builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(20);
            builder.Property(a => a.ReservedValue).HasPrecision(18, 2);
            builder.Property(a => a.UsedValue).HasPrecision(18, 2);
        }
    }
}
