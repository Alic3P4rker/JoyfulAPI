using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Joyful.API.Abstractions.Repositories;
using Joyful.API.Entities;
using Joyful.API.Models;
using Joyful.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Joyful.API.Controllers;

[Route("api/login")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IUserRepository _userRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthenticationController> _logger;
    private readonly IPasswordService _password;

    public AuthenticationController
    (
        IAuthenticationService authenticationService,
        IUserRepository userRepository,
        IAccountRepository accountRepository,
        IConfiguration configuration,
        ILogger<AuthenticationController> logger,
        IPasswordService password
    )
    {
        _authenticationService = authenticationService;
        _userRepository = userRepository;
        _accountRepository = accountRepository;
        _configuration = configuration;
        _logger = logger;
        _password = password;
    }

    [HttpPost]
    public async Task<ActionResult<TokenDto>> LoginAsync([FromBody] LoginDto login, CancellationToken cancellationToken)
    {
        //Step 1: Retrive the user 
        UserEntity? userEntity = await _userRepository.RetrieveByEmailAsync(login.EmailAddress, cancellationToken);
        if (userEntity is null)
        {
            _logger.LogError($"Unable to find user with email address: {login.EmailAddress}");
            return NotFound($"Unable to find user with email address: {login.EmailAddress}");
        }

        //Step 2: Verify password
        bool isPasswordValid = _password.VerifyPassword(login.Password, userEntity.PasswordHash);
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
        TokenDto tokenDto = await _authenticationService.CreateToken(userEntity, true);
        return Ok(tokenDto);
    }
}