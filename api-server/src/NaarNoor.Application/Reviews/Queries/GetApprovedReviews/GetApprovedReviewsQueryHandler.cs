using MediatR;
using Microsoft.EntityFrameworkCore;
using NaarNoor.Application.Common.Interfaces;

namespace NaarNoor.Application.Reviews.Queries.GetApprovedReviews;

public class GetApprovedReviewsQueryHandler : IRequestHandler<GetApprovedReviewsQuery, List<ReviewDto>>
{
    private readonly IApplicationDbContext _context;

    public GetApprovedReviewsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ReviewDto>> Handle(GetApprovedReviewsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Reviews
            .Where(r => r.IsApproved)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewDto(
                r.Id,
                r.CustomerName,
                r.Rating,
                r.Comment,
                r.Source,
                r.CreatedAt
            ))
            .ToListAsync(cancellationToken);
    }
}
