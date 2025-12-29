namespace Cirreum.Persistence;

/// <summary>
/// Paged result wrapper.
/// </summary>
public sealed record PagedResult<T>(
	IReadOnlyList<T> Items,
	int TotalCount,
	int PageSize,
	int PageNumber,
	int TotalPages);