using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NaarNoor.Domain.Entities;

namespace NaarNoor.Infrastructure.Data.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.CustomerName).IsRequired().HasMaxLength(100);
        builder.Property(r => r.Email).IsRequired().HasMaxLength(200);
        builder.Property(r => r.PhoneNumber).IsRequired().HasMaxLength(20);
        builder.Property(r => r.PartySize).IsRequired();
        builder.Property(r => r.Status).IsRequired();
        builder.Property(r => r.SpecialRequests).HasMaxLength(500);
        builder.Property(r => r.CreatedAt).IsRequired();
        builder.Property(r => r.UpdatedAt).IsRequired();
    }
}
