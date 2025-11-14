using System.ComponentModel.DataAnnotations;

namespace FeevCheckout.Dtos;

public record AuthRequest(
    [Required(ErrorMessage = "Username is required.")]
    string Username,
    [Required(ErrorMessage = "Secret is required.")]
    string Secret
);
