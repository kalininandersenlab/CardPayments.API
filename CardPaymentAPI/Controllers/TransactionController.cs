using CardPayment.Application.Transactions.Commands;
using CardPayment.Application.Transactions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardPaymentAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/transactions")]
public class TransactionController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransactionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// 4. Make a payment with a card.
    /// </summary>
    /// <param name="command">The PayWithCardCommand containing the payment details.</param>
    /// <returns>
    /// Returns a success response (200 OK) if the payment is successful, 
    /// or a BadRequest response (400) with an error message if the payment fails.
    /// </returns>
    [HttpPost("pay")]
    public async Task<IActionResult> PayWithCard([FromBody] PayWithCardCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result.ErrorMessage);
    }

    /// <summary>
    /// 5. Get the list of transactions for a specific card.
    /// </summary>
    /// <param name="cardId">The ID of the card whose transactions are to be retrieved.</param>
    /// <returns>
    /// Returns a list of transactions for the card if found, or an empty list if no transactions exist for that card.
    /// </returns>
    [HttpGet("{cardId}")]
    public async Task<IActionResult> GetTransactions(int cardId)
    {
        var transactions = await _mediator.Send(new GetTransactionsQuery(cardId));
        return Ok(transactions);
    }
}
