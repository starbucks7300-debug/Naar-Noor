using MediatR;

namespace NaarNoor.Application.Chefs.Queries.GetChefs;

public record ChefDto(
    Guid Id,
    string Name,
    string Title,
    string Bio,
    string? ImageUrl,
    string Specialty,
    int SortOrder
);

public record GetChefsQuery : IRequest<List<ChefDto>>;
