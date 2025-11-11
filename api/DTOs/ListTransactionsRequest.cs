namespace FeevCheckout.Dtos;

public record ListTransactionsRequest(
    int Page = 1,
    int PageSize = 10
);
