namespace NaarNoor.Application.Common.Models;

/// <summary>
/// Generic paged response envelope used by list queries that support pagination.
/// </summary>
public record PagedResult<T>(
    IReadOnlyList<T> Data,
    int Page,
    int PageSize,
    int Total
);
