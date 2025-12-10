namespace FeevCheckout.Controllers;

using Microsoft.AspNetCore.Mvc;

public class ExtendedController : ControllerBase
{
    protected Guid ResolveEstablishmentGuid()
    {
        var claim = User.FindFirst("id") ?? throw new UnauthorizedAccessException("Invalid token — no ID found.");

        return Guid.Parse(claim.Value);
    }
}
