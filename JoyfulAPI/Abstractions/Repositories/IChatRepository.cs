using Joyful.API.Entities;
namespace Joyful.API.Abstractions.Repositories;

public interface IChatRepository
{
    Task CreateAsync(ChatEntity chat, CancellationToken cancellationToken);
    Task DeleteAsync(ChatEntity chat, CancellationToken cancellationToken);
    Task<IEnumerable<ChatEntity>> ListChatsAsync(Guid userid, CancellationToken cancellationToken);
    Task<ChatEntity?> RetrieveChatAsync(int chatId, CancellationToken cancellationToken);
    Task<ChatEntity?> FindChatbyParticipantsAsync(Guid creatorId, List<Guid> participantIds, CancellationToken cancellationToken);
    Task<ChatEntity?> FindOneOnOneChatAsync(Guid userId1, Guid userId2, CancellationToken cancellationToken);
    Task UpdateAsync(ChatEntity chat, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}