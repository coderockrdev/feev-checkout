using System.Text.Json;

using FeevCheckout.Enums;
using FeevCheckout.Services.Webhooks;

using Microsoft.AspNetCore.Mvc;

namespace FeevCheckout.Controllers;

[ApiController]
[Route("/webhook")]
public class WebhookController(WebhookProcessorFactory webhookProcessorFactory) : ControllerBase
{
    private readonly WebhookProcessorFactory _webhookProcessorFactory = webhookProcessorFactory;

    [HttpPost("{method}")]
    public async Task<IActionResult> Index(PaymentMethod method, [FromBody] JsonElement payload)
    {
        var processor = _webhookProcessorFactory.GetProcessor(method) ??
                        throw new InvalidOperationException($"No processor registered for '{method}'.");

        var result = await processor.ProcessAsync(payload);

        return Ok(result);
    }
}
