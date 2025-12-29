namespace FeevCheckout.DTOs;

public record ListTransactionsRequest(
    int Page = 1,
    int PageSize = 10
);
