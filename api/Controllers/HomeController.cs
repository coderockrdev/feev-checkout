using Microsoft.AspNetCore.Mvc;

namespace FeevCheckout.Controllers;

[ApiController]
[Route("/")]
public class HomeController : ControllerBase
{
    [HttpGet]
    public IActionResult Index()
    {
        return Ok(new { status = "API is running." });
    }
}
