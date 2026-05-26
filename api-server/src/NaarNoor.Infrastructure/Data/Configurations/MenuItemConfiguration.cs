using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NaarNoor.Domain.Entities;

namespace NaarNoor.Infrastructure.Data.Configurations;

public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Name).IsRequired().HasMaxLength(150);
        builder.Property(m => m.Description).IsRequired().HasMaxLength(500);
        builder.Property(m => m.Price).IsRequired().HasColumnType("decimal(10,2)");
        builder.Property(m => m.Category).IsRequired();
        builder.Property(m => m.ImageUrl).HasMaxLength(500);
        builder.Property(m => m.CreatedAt).IsRequired();
        builder.Property(m => m.UpdatedAt).IsRequired();
    }
}
