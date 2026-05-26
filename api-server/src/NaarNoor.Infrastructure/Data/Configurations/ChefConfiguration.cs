using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NaarNoor.Domain.Entities;

namespace NaarNoor.Infrastructure.Data.Configurations;

public class ChefConfiguration : IEntityTypeConfiguration<Chef>
{
    public void Configure(EntityTypeBuilder<Chef> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Title).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Bio).IsRequired().HasMaxLength(1000);
        builder.Property(c => c.ImageUrl).HasMaxLength(500);
        builder.Property(c => c.Specialty).IsRequired().HasMaxLength(200);
        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.UpdatedAt).IsRequired();
    }
}
