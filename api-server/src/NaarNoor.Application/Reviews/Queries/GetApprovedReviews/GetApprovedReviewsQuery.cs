using MediatR;

namespace NaarNoor.Application.Reviews.Queries.GetApprovedReviews;

public record ReviewDto(
    Guid Id,
    string CustomerName,
    int Rating,
    string Comment,
    string? Source,
    DateTime CreatedAt
);

public record GetApprovedReviewsQuery : IRequest<List<ReviewDto>>;
