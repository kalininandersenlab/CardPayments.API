using CardPayment.Application.Cards.Commands;
using CardPayment.Application.Cards.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardPaymentAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/cards")]
    public class CardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// 1. Create a new card.
        /// </summary>
        /// <returns>
        /// Returns a response with the created card details and status 201 (Created) along with the location of the card resource.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> CreateCard()
        {
            var result = await _mediator.Send(new CreateCardCommand());
            return CreatedAtAction(nameof(GetCardBalance), new { cardId = result.Id }, result);
        }

        /// <summary>
        /// 2. Get the balance of a card by its ID.
        /// </summary>
        /// <param name="cardId">The ID of the card whose balance is to be retrieved.</param>
        /// <returns>
        /// Returns the balance of the card if found, or a NotFound response if the card does not exist.
        /// </returns>
        [HttpGet("{cardId}/balance")]
        public async Task<IActionResult> GetCardBalance(int cardId)
        {
            var result = await _mediator.Send(new GetCardBalanceQuery(cardId));
            return result != null ? Ok(result) : NotFound("Card not found");
        }

        /// <summary>
        /// 3. Update the details of an existing card.
        /// </summary>
        /// <param name="cardId">The ID of the card to be updated.</param>
        /// <param name="command">The UpdateCardCommand object containing the updated details of the card.</param>
        /// <returns>
        /// Returns a success status (200 OK) if the card was updated successfully, 
        /// or a NotFound status if the card with the specified ID was not found.
        /// </returns>
        [HttpPut("{cardId}")]
        public async Task<IActionResult> UpdateCard(int cardId, [FromBody] UpdateCardCommand command)
        {
            command.CardId = cardId;
            var result = await _mediator.Send(command);
            return result ? Ok() : NotFound("Card not found");
        }
    }
}
