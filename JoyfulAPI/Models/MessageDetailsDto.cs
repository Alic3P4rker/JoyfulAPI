using Joyful.API.Enums;

namespace Joyful.API.Models;

public record MessageDetailsDto
{
    public int Id { get; init; }
    public string Content { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public Guid SenderId { get; init; } = Guid.Empty;
    public MessageStatus Status { get; init; } = MessageStatus.Sent;
}