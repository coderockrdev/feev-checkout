using System.ComponentModel.DataAnnotations;

using FeevCheckout.Dtos;
using FeevCheckout.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FeevCheckout.Controllers;

[ApiController]
[Route("/transaction")]
public class TransactionController(ITransactionService transactionService) : ExtendedController
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> ListTransactions(
        [FromQuery] ListTransactionsRequest request)
    {
        var establishmentId = ResolveEstablishmentGuid();

        if (request.Page <= 0 || request.PageSize <= 0)
            return BadRequest(new { message = "page and pageSize must be greater than 0." });

        var result = await transactionService.ListTransactions(establishmentId, request.Page, request.PageSize);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTransaction(Guid id)
    {
        var transaction = await transactionService.GetPublicTransaction(id);

        if (transaction == null)
            return NotFound(new { message = "Transaction not found." });

        return Ok(transaction);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody][Required] CreateTransactionRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var establishmentId = ResolveEstablishmentGuid();
        var transaction = await transactionService.CreateTransaction(establishmentId, request);

        return Ok(transaction);
    }

    [Authorize]
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> CancelTransaction(Guid id)
    {
        var establishmentId = ResolveEstablishmentGuid();
        var success = await transactionService.CancelTransaction(establishmentId, id);

        if (!success)
            return NotFound(new { message = "Transaction not found or already canceled." });

        return Ok(new { message = "Transaction canceled successfully." });
    }
}
