namespace FeevCheckout.DTOs;

public sealed class PagedResult<T>
{
    public required int Page { get; set; }

    public required int PageSize { get; set; }

    public required int TotalCount { get; set; }

    public required int TotalPages { get; set; }

    public required List<T> Data { get; set; }
}
