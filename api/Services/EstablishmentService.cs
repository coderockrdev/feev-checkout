using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

    Task<Establishment?> GetEstablishment(string username);
}

public class EstablishmentService(AppDbContext context, IConfiguration configuration) : IEstablishmentService
{
    private readonly AppDbContext context = context;

    private readonly string jwtKey = configuration["AppSettings:JwtKey"]
                                     ?? throw new InvalidOperationException("JWT Key not found or not specified");

    private readonly string secretKey = configuration["AppSettings:SecretKey"]
                                        ?? throw new InvalidOperationException("Secret Key not found or not specified");

    public bool ValidateSecret(Establishment establishment, string input)
    {
        var hash = HmacValidator.Compute(establishment.Username, secretKey);

        return HmacValidator.Validate(hash, input);
    }

    public string GenerateJwt(Establishment establishment)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("id", establishment.Id.ToString()),
            new Claim("username", establishment.Username),
            new Claim("name", establishment.Name)
        };

        var token = new JwtSecurityToken(
            "Feev Checkout",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<Establishment?> GetEstablishment(string username)
    {
        return await context.Establishments
            .FirstOrDefaultAsync(establishment => establishment.Username == username);
    }
}
