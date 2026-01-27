using System.ComponentModel.DataAnnotations;

using FeevCheckout.DTOs;
using FeevCheckout.Services;

using Microsoft.AspNetCore.Mvc;

namespace FeevCheckout.Controllers;

[ApiController]
[Route("/payment")]
public class PaymentController(ITransactionService transactionService, IPaymentService paymentService)
    : ExtendedController
{
    [HttpPost("{id:guid}")]
    public async Task<IActionResult> Index(Guid id, [FromBody][Required] PaymentRequestDto request)
    {
        var transaction = await transactionService.GetPublicTransaction(id);

        if (transaction == null)
            throw new KeyNotFoundException("Transaction not found.");

        var result = await paymentService.Process(transaction, request);

        return Ok(result);
    }
}
