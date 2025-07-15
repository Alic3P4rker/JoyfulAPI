using Joyful.API.Enums;

namespace Joyful.API.Models;

public record PlannerDto(
    string displayName,
    string firstName,
    string lastName,
    Gender gender,
    DateOnly dob,
    string emailAddress,
    string telephoneNumber,
    Status status,
    Role role
);