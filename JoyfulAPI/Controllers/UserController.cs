using System.Security.Cryptography;
using AutoMapper;
using Joyful.API.Abstractions.Repositories;
using Joyful.API.Entities;
using Joyful.API.Models;
using Joyful.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Joyful.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserController> _logger;
    private readonly IPasswordService _password;

    public UserController(
        IAccountRepository accountRepository,
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<UserController> logger,
        IPasswordService password
    )
    {
        _accountRepository = accountRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
        _password = password;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUserAsync([FromBody] UserCreateDto user, CancellationToken cancellationToken)
    {
        //Step 1: Check if user already exists
        UserEntity? tempUserEntity = await _userRepository.RetrieveByEmailAsync(user.EmailAddress, cancellationToken);
        if (tempUserEntity is not null)
        {
            _logger.LogWarning("Attempting to create user with an already exisiting email");
            return Conflict("A user with this email already exists");
        }
            

        //Step 2: Create user
        UserEntity userEntity = _mapper.Map<UserEntity>(user);
        userEntity.PasswordHash = _password.HashPassword(user.Password);
        await _userRepository.CreateAsync(userEntity, cancellationToken);

        //Step 3: Create user account
        string otp = RandomNumberGenerator.GetInt32(000000, 999999).ToString("D6");
        AccountEntity accountEntity = new AccountEntity();
        accountEntity.Id = userEntity.Id;
        accountEntity.Otp = otp;
        accountEntity.CreatedAt = DateTime.UtcNow;
        accountEntity.ExpiresAt = DateTime.UtcNow.AddMinutes(30);

        //Step 4: Save Changes
        await _accountRepository.CreateAsync(accountEntity, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        UserDetailsDto userDetailsDto = _mapper.Map<UserDetailsDto>(userEntity);

#warning CreatedAtAction not working so temp measure in place
        return StatusCode(StatusCodes.Status201Created, userDetailsDto);
    }

    [HttpGet("{Id:guid}", Name = "GetUserById")]
    public async Task<ActionResult<UserDetailsDto>> RetrieveUserByIdAsync(Guid Id, CancellationToken cancellationToken)
    {
        UserEntity? userEntity = await _userRepository.RetrieveByIdAsync(Id, cancellationToken);
        if (userEntity is null)
        {
            _logger.LogWarning("Retrieve by Id failed: User with Id {UserId} not found.", Id);
            return NotFound($"User with id {Id} was not found");
        }

        UserDetailsDto user = _mapper.Map<UserDetailsDto>(userEntity);
        return Ok(user);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUserAsync(Guid id, CancellationToken cancellationToken)
    {
        UserEntity? userEntity = await _userRepository.RetrieveByIdAsync(id, cancellationToken);
        if (userEntity is null)
            return NotFound($"User with id {id} was not found");

        await _userRepository.DeleteAsync(userEntity, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpPut("{emailAddress}")]
    public async Task<IActionResult> UpdateUserAsync(string emailAddress, [FromBody] UserCreateDto user, CancellationToken cancellationToken)
    {
        if (emailAddress != user.EmailAddress)
            return BadRequest("Email addresses don't match");

        UserEntity? userEntity = await _userRepository.RetrieveByEmailAsync(emailAddress, cancellationToken);

        if (userEntity is null)
            return NotFound($"User with email address {emailAddress} was not found");

        _mapper.Map(user, userEntity);

        if (!string.IsNullOrEmpty(user.EmailAddress))
            userEntity.PasswordHash = _password.HashPassword(user.Password);


        await _userRepository.UpdateAsync(userEntity, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
    
