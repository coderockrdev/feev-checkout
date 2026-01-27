using Microsoft.AspNetCore.Mvc;

namespace FeevCheckout.Controllers;

public class ExtendedController : ControllerBase
{
    protected Guid ResolveEstablishmentGuid()
    {
        var claim = User.FindFirst("id");

        if (claim is null || !Guid.TryParse(claim.Value, out var id))
            throw new UnauthorizedAccessException("Invalid token — ID is missing or invalid.");

        return id;
    }
}
