using System.Security.Claims;
using ECommerce.Application.Abstractions;
using ECommerce.Contracts.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authService;

    public AuthenticationController(IAuthenticationService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);
        if (result.IsFailure)
        {
            return Problem(result.Error.Description, statusCode: result.Error.Type switch
            {
                Domain.Primitives.ErrorType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status400BadRequest
            });
        }

        return Ok(result.Value);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);
        if (result.IsFailure)
        {
            return Problem(result.Error.Description, statusCode: StatusCodes.Status401Unauthorized);
        }

        return Ok(result.Value);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshTokenAsync(request, cancellationToken);
        if (result.IsFailure)
        {
            return Problem(result.Error.Description, statusCode: StatusCodes.Status401Unauthorized);
        }

        return Ok(result.Value);
    }

    [Authorize]
    [HttpPost("revoke")]
    public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RevokeTokenAsync(request, cancellationToken);
        if (result.IsFailure)
        {
            return Problem(result.Error.Description, statusCode: StatusCodes.Status404NotFound);
        }

        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var result = await _authService.GetCurrentUserAsync(userId, cancellationToken);
        if (result.IsFailure)
        {
            return NotFound(result.Error.Description);
        }

        return Ok(result.Value);
    }
}
