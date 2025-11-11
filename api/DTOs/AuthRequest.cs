using System.ComponentModel.DataAnnotations;

namespace FeevCheckout.Dtos;

public record AuthRequest(
    [Required] string Username,
    [Required] string Secret
);
