using FeevCheckout.Data;
using FeevCheckout.Models;
using FeevCheckout.Utils;

using Microsoft.EntityFrameworkCore;

namespace FeevCheckout.Services;

public interface ICardBrandPatternService
{
    Task<CardBrandPattern?> GetByCardNumber(string cardNumber);
}

public class CardBrandPatternService(AppDbContext context) : ICardBrandPatternService
{
    private readonly AppDbContext context = context;

    public async Task<CardBrandPattern?> GetByCardNumber(string cardNumber)
    {
        if (CardUtils.IsTesting(cardNumber))
            return await context.CardBrandPatterns
                .OrderBy(pattern => pattern.Order)
                .FirstOrDefaultAsync();

        return await context.CardBrandPatterns
            .Where(pattern => cardNumber.StartsWith(pattern.Prefix))
            .OrderBy(pattern => pattern.Order)
            .FirstOrDefaultAsync();
    }
}
