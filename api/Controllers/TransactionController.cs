using System.ComponentModel.DataAnnotations;
using FeevCheckout.Dtos;
using FeevCheckout.Services;
using Microsoft.AspNetCore.Mvc;

namespace FeevCheckout.Controllers;

[ApiController]
[Route("/transaction")]
public class TransactionController(ITransactionService transactionService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ListTransactions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (page <= 0 || pageSize <= 0)
            return BadRequest(new { message = "Page and pageSize must be greater than 0." });

        var result = await transactionService.ListTransactions(page, pageSize);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody, Required] CreateTransactionRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var transaction = await transactionService.CreateTransaction(request);

        return Ok(transaction);
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> CancelTransaction(Guid id)
    {
        var success = await transactionService.CancelTransaction(id);

        if (!success)
            return NotFound(new { message = "Transaction not found or already canceled." });

        return Ok(new { message = "Transaction canceled successfully." });
    }
}
