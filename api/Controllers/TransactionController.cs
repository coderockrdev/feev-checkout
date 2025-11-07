using System.ComponentModel.DataAnnotations;
using FeevCheckout.Dtos;
using FeevCheckout.Services;
using Microsoft.AspNetCore.Mvc;

namespace FeevCheckout.Controllers;

[ApiController]
[Route("/transaction")]
public class TransactionController(ITransactionService transactionService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody, Required] CreateTransactionRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var transaction = await transactionService.CreateTransactionAsync(request);

        return Ok(transaction);
    }
}
