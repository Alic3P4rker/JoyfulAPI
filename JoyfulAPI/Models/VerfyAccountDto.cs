namespace Joyful.API.Models;

public record VerifyAccountDto(
    Guid id,
    string otp
);