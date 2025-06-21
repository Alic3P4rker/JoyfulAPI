using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Joyful.API.Abstractions.Repositories;
using Joyful.API.Entities;
using Joyful.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Joyful.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController
    (
        IUserRepository userRepository,
        IAccountRepository accountRepository,
        IConfiguration configuration,
        ILogger<AuthenticationController> logger
    )
    {
        _userRepository = userRepository;
        _accountRepository = accountRepository;
        _configuration = configuration;
        _logger = logger;
    }
    
    [HttpPost]
    public async Task<ActionResult<string>> LoginAsync([FromBody] LoginDto login, CancellationToken cancellationToken)
    {
        //Step 1: Retrive the user 
        UserEntity? userEntity = await _userRepository.RetrieveByEmailAsync(login.EmailAddress, cancellationToken);
        if (userEntity is null)
        {
            _logger.LogError($"Unable to find user with email address: {login.EmailAddress}");
            return NotFound($"Unable to find user with email address: {login.EmailAddress}");
        }

        //Step 2: Verify password
        bool isPasswordValid = true;
        if (!isPasswordValid)
        {
            _logger.LogInformation($"Invalid details");
            return Unauthorized($"Invalid details");
        }

        //Step 3: Verify account status
        AccountEntity? accountEntity = await _accountRepository.RetrieveAsync(userEntity.Id, cancellationToken);
        if (accountEntity is null || accountEntity.IsVerified == false)
        {
            _logger.LogError($"Unable to find account of email address: {login.EmailAddress} or Account is not verfied");
            return NotFound("Invalid details");
        }

        //Step 4: Generate jwt
        var securityKey = new SymmetricSecurityKey(
            Convert.FromBase64String(_configuration["Authentication:SecretForKey"]));

        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claimsForToken = new List<Claim>();
        claimsForToken.Add(new Claim("sub", userEntity.Id.ToString()));
        claimsForToken.Add(new Claim("email", userEntity.EmailAddress));

        var jwtSecurityToken = new JwtSecurityToken(
            _configuration["Authentication:Issuer"],
            _configuration["Authentication:Audience"],
            claimsForToken,
            DateTime.UtcNow,
#warning long time for now replace with shorter time and with refresh tokens
            DateTime.UtcNow.AddMinutes(15),
            signingCredentials
        );

        var tokenToReturn = new JwtSecurityTokenHandler()
            .WriteToken(jwtSecurityToken);

        return Ok(tokenToReturn);

    }
}