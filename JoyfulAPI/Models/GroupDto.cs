using Joyful.API.Enums;

namespace Joyful.API.Models;

public record GroupDto(
    Guid id,
    string name,
    Guid groupLeaderId,
    Status status,
    bool isPrivate,
    bool isOrganisation,
    bool requiresApproval
);