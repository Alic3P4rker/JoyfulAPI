using Joyful.API.Entities;
namespace Joyful.API.Abstractions.Repositories;

public interface IMessageRepository
{
    Task CreateAsync(MessageEntity message, CancellationToken cancellationToken);
    Task DeleteAsync(MessageEntity message, CancellationToken cancellationToken);
    Task<MessageEntity?> RetrieveMessageByIdAsync(int messageId, CancellationToken cancellationToken);
    Task<IEnumerable<MessageEntity>> SearchMessagesAsync(int chatId, string searchTerm, CancellationToken cancellationToken);
    Task<IEnumerable<MessageEntity>> GetMessagesByChatIdAsync(int chatId, CancellationToken cancellationToken);
    Task UpdateAsync(MessageEntity message, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}