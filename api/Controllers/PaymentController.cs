using System.ComponentModel.DataAnnotations;

using FeevCheckout.Dtos;
using FeevCheckout.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FeevCheckout.Controllers;

[ApiController]
[Route("/payment")]
[Authorize]
public class PaymentController(ITransactionService transactionService, IPaymentService paymentService)
    : ExtendedController
{
    [HttpPost("{id:guid}")]
    public async Task<IActionResult> Index(Guid id, [FromBody] [Required] PaymentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var establishmentId = ResolveEstablishmentGuid();
        var transaction = await transactionService.GetTransaction(establishmentId, id);

        if (transaction == null)
            return NotFound(new { message = "Transaction not found." });

        var result = await paymentService.Process(transaction, request.Method, request.Installments);

        return Ok(result);
    }
}
