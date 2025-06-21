using Joyful.API.Entities;

namespace Joyful.API.Abstractions.Repositories;

public interface IEventRepository
{
    Task CreateAsync(EventEntity eventEntity, CancellationToken cancellationToken);
    Task DeleteAsync(EventEntity eventEntity, CancellationToken cancellationToken);
    Task<IEnumerable<EventEntity>> ListAsync(CancellationToken cancellationToken);
    Task<EventEntity?> RetrieveAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateAsync(EventEntity eventEntity, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}