using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NaarNoor.Domain.Entities;

namespace NaarNoor.Infrastructure.Data.Configurations;

public class ContactInquiryConfiguration : IEntityTypeConfiguration<ContactInquiry>
{
    public void Configure(EntityTypeBuilder<ContactInquiry> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Email).IsRequired().HasMaxLength(200);
        builder.Property(c => c.PhoneNumber).HasMaxLength(20);
        builder.Property(c => c.Subject).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Message).IsRequired().HasMaxLength(2000);
        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.UpdatedAt).IsRequired();
    }
}
