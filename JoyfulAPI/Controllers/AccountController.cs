using System.Security.Cryptography;
using AutoMapper;
using Joyful.API.Abstractions.Repositories;
using Joyful.API.Entities;
using Joyful.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Joyful.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;
    private ILogger<AccountController> _logger;

    public AccountController(IAccountRepository accountRepository, IMapper mapper, ILogger<AccountController> logger)
    {
        _accountRepository = accountRepository;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPost("verify")]
    public async Task<ActionResult> VerifyAccountAsync([FromBody] VerifyAccountDto request, CancellationToken cancellationToken)
    {
        AccountEntity? accountEntity = await _accountRepository.RetrieveAsync(request.id, cancellationToken);
        if (accountEntity is null
            || accountEntity.Otp == null
            || accountEntity.ExpiresAt < DateTime.UtcNow
            || accountEntity.Otp != request.otp
        )
        {
            _logger.LogInformation($"Account verification for id; {request.id} failed");
            return Unauthorized("Invalid or Expired OTP");
        }

        if (accountEntity.IsVerified == true)
        {
            _logger.LogInformation($"Account {request.id} is already verified, no more action needed");
            return Ok("Account already verfied, no action needed");
        }

        accountEntity.Otp = null;
        accountEntity.IsVerified = true;

        await _accountRepository.UpdateAsync(accountEntity, cancellationToken);
        await _accountRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation($"Account verification for id; {request.id} successfully verified");
        return Ok("Account verified successfully");
    }

    [HttpPost("{id:guid}/generate-otp")]
    public async Task<ActionResult> GenerateOTPAsync(Guid id, CancellationToken cancellationToken)
    {
        AccountEntity? accountEntity = await _accountRepository.RetrieveAsync(id, cancellationToken);
        if (
            accountEntity is null
            || accountEntity.IsVerified == true)
        {
            _logger.LogInformation($"Unable to find account with {id} or account already verified");
            return NotFound("Account not found or Account already verified");
        }

        string otp = RandomNumberGenerator.GetInt32(000000, 999999).ToString("D6");
        accountEntity.Otp = otp;
        accountEntity.CreatedAt = DateTime.UtcNow;
        accountEntity.ExpiresAt = DateTime.UtcNow.AddMinutes(30);

        await _accountRepository.UpdateAsync(accountEntity, cancellationToken);
        await _accountRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation($"Otp for account {accountEntity.Id} successfully generated");
        return Ok("Otp successfully generated");
    }
}