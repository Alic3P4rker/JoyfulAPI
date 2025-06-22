using Joyful.API.Enums;

namespace Joyful.API.Models;

public record LocationDto(
    Guid id,
    Guid plannerId,
    string address1,
    string address2,
    County county,
    string city,
    string postCode
);