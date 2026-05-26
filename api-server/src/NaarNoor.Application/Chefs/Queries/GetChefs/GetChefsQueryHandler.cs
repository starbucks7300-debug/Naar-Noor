using MediatR;
using Microsoft.EntityFrameworkCore;
using NaarNoor.Application.Common.Interfaces;

namespace NaarNoor.Application.Chefs.Queries.GetChefs;

public class GetChefsQueryHandler : IRequestHandler<GetChefsQuery, List<ChefDto>>
{
    private readonly IApplicationDbContext _context;

    public GetChefsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ChefDto>> Handle(GetChefsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Chefs
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder)
            .Select(c => new ChefDto(
                c.Id,
                c.Name,
                c.Title,
                c.Bio,
                c.ImageUrl,
                c.Specialty,
                c.SortOrder
            ))
            .ToListAsync(cancellationToken);
    }
}
