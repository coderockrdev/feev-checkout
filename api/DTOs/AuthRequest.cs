using System.ComponentModel.DataAnnotations;

namespace FeevCheckout.Dtos;

public record AuthRequest(
    [Required(ErrorMessage = "Client ID is required.")]
    string ClientId,
    [Required(ErrorMessage = "Client secret is required.")]
    string ClientSecret
);
