using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Joyful.API.Abstractions.Repositories;
using Joyful.API.Entities;
using Joyful.API.Models;
using Microsoft.IdentityModel.Tokens;

namespace Joyful.API.Services;

public interface IAuthenticationService
{
    public Task<TokenDto> CreateToken(UserEntity userEntity, bool populateExp);
    public string GenerateRefreshToken(bool populateExp);
    public ClaimsPrincipal GetClaimsPrincipal(string token);
    Task<TokenDto> RefreshToken(TokenDto tokenDto);
}

internal sealed class AuthenticationService : IAuthenticationService
{
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;

    public AuthenticationService(IConfiguration configuration, IUserRepository userRepository)
    {
        _configuration = configuration
            ?? throw new ArgumentNullException(nameof(configuration));
        _userRepository = userRepository
            ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<TokenDto> CreateToken(UserEntity userEntity, bool populateExp)
    {  
        var securityKey = new SymmetricSecurityKey(
            Convert.FromBase64String(_configuration["Authentication:SecretForKey"]));

        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claimsForToken = new List<Claim>();
        claimsForToken.Add(new Claim("sub", userEntity.Id.ToString()));
        claimsForToken.Add(new Claim("emailAddress", userEntity.EmailAddress));

        var jwtSecurityToken = new JwtSecurityToken(
            _configuration["Authentication:Issuer"],
            _configuration["Authentication:Audience"],
            claimsForToken,
            DateTime.UtcNow,
            _configuration["Authentication:AccessTokenExpirationMinutes"] is not null
                ? DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Authentication:AccessTokenExpirationMinutes"]))
                : DateTime.UtcNow.AddMinutes(10),
            signingCredentials
        );

        var refreshToken = GenerateRefreshToken(true);
        userEntity.RefreshToken = refreshToken;
        if (populateExp)
         userEntity.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _userRepository.UpdateAsync(userEntity, CancellationToken.None);
        await _userRepository.SaveChangesAsync(CancellationToken.None);

        var accessToken = new JwtSecurityTokenHandler()
            .WriteToken(jwtSecurityToken);


        return new TokenDto(accessToken, refreshToken);  
    }

    public string GenerateRefreshToken(bool populateExp)
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    public ClaimsPrincipal GetClaimsPrincipal(string token)
    {
        var jwtSettings = _configuration.GetSection("Authentication");
        var tokenValidationSettings = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Convert.FromBase64String(_configuration["Authentication:SecretForKey"])
            ),
            ValidateLifetime = false,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationSettings, out securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null
            || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }

    public async Task<TokenDto> RefreshToken(TokenDto tokenDto)
    {
        var principal = GetClaimsPrincipal(tokenDto.AccessToken);
        var emailAddress = principal.FindFirstValue("emailAddress");
        var user = _userRepository.RetrieveByEmailAsync(emailAddress, CancellationToken.None).Result;
        if (user == null
            || user.RefreshToken != tokenDto.RefreshToken
            || user.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            throw new SecurityTokenException("Invalid refresh token");
        }

        UserEntity userEntity = user;
        return await CreateToken(userEntity, false);
    }
}