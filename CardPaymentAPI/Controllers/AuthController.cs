using CardPayment.Application.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CardPaymentAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="command">The RegisterCommand object containing user registration details.</param>
    /// <returns>
    /// Returns a success message with a token and refresh token on successful registration.
    /// Returns a BadRequest with the error message if registration fails.
    /// </returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(new { token = result.Token, refreshToken = result.RefreshToken });
    }

    /// <summary>
    /// Logs in an existing user.
    /// </summary>
    /// <param name="command">The LoginCommand object containing login credentials (username and password).</param>
    /// <returns>
    /// Returns a success message with a token and refresh token on successful login.
    /// Returns Unauthorized with the error message if login fails.
    /// </returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return Unauthorized(result.Message);

        return Ok(new { token = result.Token, refreshToken = result.RefreshToken });
    }

    /// <summary>
    /// Refreshes the JWT token using a valid refresh token.
    /// </summary>
    /// <param name="command">The RefreshTokenCommand object containing the refresh token.</param>
    /// <returns>
    /// Returns a new token and refresh token if the refresh token is valid.
    /// Returns Unauthorized with an error message if the refresh token is invalid.
    /// </returns>
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return Unauthorized(result.Message);

        return Ok(new { token = result.Token, refreshToken = result.RefreshToken });
    }
}