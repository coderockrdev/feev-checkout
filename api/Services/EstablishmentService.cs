using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using FeevCheckout.Data;
using FeevCheckout.Models;
using FeevCheckout.Utils;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FeevCheckout.Services;

public interface IEstablishmentService
{
    bool ValidateSecret(Establishment establishment, string secret);

    string GenerateJwt(Establishment establishment);

    string GenerateClientId(int size);

    Task<Establishment?> GetEstablishment(Guid id);

    Task<Establishment?> GetEstablishmentByClientId(string clientID);

    Task<Establishment> CreateEstablishment(
        string fullName,
        string shortName,
        string cnpj,
        string domain,
        string? bankNumber,
        string? bankAgency,
        string? bankAccount,
        string? checkingAccountNumber,
        string? creditCardProvider
    );
}

public class EstablishmentService(AppDbContext context, IConfiguration configuration) : IEstablishmentService
{
    private readonly AppDbContext context = context;

    private readonly string jwtKey = configuration["AppSettings:JwtKey"]
                                     ?? throw new InvalidOperationException("JWT Key not found or not specified.");

    private readonly string secretKey = configuration["AppSettings:SecretKey"]
                                        ?? throw new InvalidOperationException(
                                            "Secret Key not found or not specified.");

    public bool ValidateSecret(Establishment establishment, string input)
    {
        var hash = HmacValidator.Compute(establishment.ClientId, secretKey);

        return HmacValidator.Validate(hash, input);
    }

    public string GenerateJwt(Establishment establishment)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("id", establishment.Id.ToString()),
            new Claim("client_id", establishment.ClientId),
            new Claim("name", establishment.FullName)
        };

        var token = new JwtSecurityToken(
            "Feev Checkout",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateClientId(int size = 20)
    {
        var bytes = RandomNumberGenerator.GetBytes(size);

        return Convert.ToBase64String(bytes)
            .Replace("+", "")
            .Replace("/", "")
            .Replace("=", "");
    }

    public async Task<Establishment?> GetEstablishment(Guid id)
    {
        return await context.Establishments
            .FirstOrDefaultAsync(establishment => establishment.Id == id);
    }

    public async Task<Establishment?> GetEstablishmentByClientId(string clientId)
    {
        return await context.Establishments
            .FirstOrDefaultAsync(establishment => establishment.ClientId == clientId);
    }

    public async Task<Establishment> CreateEstablishment(
        string fullName,
        string shortName,
        string cnpj,
        string domain,
        string? bankNumber,
        string? bankAgency,
        string? bankAccount,
        string? checkingAccountNumber,
        string? creditCardProvider
    )
    {
        var id = GenerateClientId(12);
        var secret = HmacValidator.Compute(id, secretKey);

        var establishment = new Establishment
        {
            Id = Guid.NewGuid(),
            FullName = fullName,
            ShortName = shortName,
            CNPJ = cnpj,
            Domain = domain,
            BankNumber = bankNumber,
            BankAgency = bankAgency,
            BankAccount = bankAccount,
            CheckingAccountNumber = checkingAccountNumber,
            ClientId = id,
            ClientSecret = secret
        };

        context.Establishments.Add(establishment);
        await context.SaveChangesAsync();

        return establishment;
    }
}
