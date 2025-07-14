using Joyful.API.Entities;
namespace Joyful.API.Abstractions.Repositories;

public interface IMessageRepository
{
    Task CreateAsync(MessageEntity message, CancellationToken cancellationToken);
    Task DeleteAsync(MessageEntity message, CancellationToken cancellationToken);
    Task<IEnumerable<MessageEntity>> ListAsync(CancellationToken cancellationToken);
    Task<MessageEntity?> RetrieveAsync(int id, CancellationToken cancellationToken);
    Task UpdateAsync(MessageEntity message, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}