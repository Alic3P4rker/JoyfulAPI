using Joyful.API.Models;
using Joyful.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Joyful.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TokenController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public TokenController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService
            ?? throw new ArgumentNullException(nameof(authenticationService));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] TokenDto tokenDto)
    {
        var tokenToReturn = await _authenticationService.RefreshToken(tokenDto);
        return Ok(tokenToReturn);
    }

}