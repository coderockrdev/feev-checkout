using System.ComponentModel.DataAnnotations;

using FeevCheckout.Dtos;
using FeevCheckout.Services;

using Microsoft.AspNetCore.Mvc;

namespace FeevCheckout.Controllers;

[ApiController]
[Route("/payment")]
public class PaymentController(ITransactionService transactionService, IPaymentService paymentService)
    : ExtendedController
{
    [HttpPost("{id:guid}")]
    public async Task<IActionResult> Index(Guid id, [FromBody] [Required] PaymentRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var transaction = await transactionService.GetPublicTransaction(id);

        if (transaction == null)
            return NotFound(new { message = "Transaction not found." });

        var result = await paymentService.Process(transaction, request);

        return Ok(result);
    }
}
