using Microsoft.EntityFrameworkCore;
using NaarNoor.Domain.Entities;

namespace NaarNoor.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Reservation> Reservations { get; }
    DbSet<MenuItem> MenuItems { get; }
    DbSet<Chef> Chefs { get; }
    DbSet<Review> Reviews { get; }
    DbSet<ContactInquiry> ContactInquiries { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
