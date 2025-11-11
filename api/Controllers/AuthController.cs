using FeevCheckout.Dtos;
using FeevCheckout.Services;

using Microsoft.AspNetCore.Mvc;

namespace FeevCheckout.Controllers;

[ApiController]
[Route("/auth")]
public class AuthController(IEstablishmentService establishmentService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Index([FromBody] AuthRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var establishment = await establishmentService.GetEstablishment(request.Username);

        if (establishment == null)
            return NotFound(new { message = "Establishment not found." });

        var isValid = false;

        try
        {
            isValid = establishmentService.ValidateSecret(establishment, request.Secret);
        }
        catch (FormatException)
        {
            // It means it's a non-valid secret
        }

        if (!isValid)
            return Unauthorized(new { message = "Invalid secret." });

        var jwt = establishmentService.GenerateJwt(establishment);

        return Ok(new { token = jwt });
    }
}
