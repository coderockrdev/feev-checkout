using FeevCheckout.DTOs;
using FeevCheckout.Services;

using Microsoft.AspNetCore.Mvc;

namespace FeevCheckout.Controllers;

[ApiController]
[Route("/auth")]
public class AuthController(IEstablishmentService establishmentService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<AuthResponseDto>> Index([FromBody] AuthRequest request)
    {
        var establishment = await establishmentService.GetEstablishmentByClientId(request.ClientId);

        if (establishment == null)
            throw new KeyNotFoundException("Establishment not found.");

        var isValid = false;

        try
        {
            isValid = establishmentService.ValidateSecret(establishment, request.ClientSecret);
        }
        catch (FormatException)
        {
            // It means it's a non-valid secret
        }

        if (!isValid)
            throw new UnauthorizedAccessException("Invalid secret.");

        var jwt = establishmentService.GenerateJwt(establishment);

        return Ok(new AuthResponseDto(jwt));
    }
}
