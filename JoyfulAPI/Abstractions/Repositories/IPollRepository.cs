using Joyful.API.Entities;

namespace Joyful.API.Abstractions.Repositories;

public interface IPollRepository
{
    Task CreateAsync(PollEntity pollEntity, CancellationToken cancellationToken);
    Task DeleteAsync(PollEntity pollEntity, CancellationToken cancellationToken);
    Task<IEnumerable<PollEntity>> ListAsync(CancellationToken cancellationToken);
    Task<PollEntity> RetrieveAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateAsync(PollEntity pollEntity, CancellationToken cancellationToken);
}