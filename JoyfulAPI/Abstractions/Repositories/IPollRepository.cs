using Joyful.API.Entities;

namespace Joyful.API.Abstractions.Repositories;

public interface IPollRepository
{
    Task CreateAsync(PollEntity pollEntity, CancellationToken cancellationToken);
    Task DeleteAsync(PollEntity pollEntity, CancellationToken cancellationToken);
    Task<IEnumerable<PollEntity>> ListPollsForEventAsync(Guid id, CancellationToken cancellationToken);
    Task<PollEntity?> RetrievePollAsync(Guid id, CancellationToken cancellationToken);
    Task<PollEntity?> RetrievePollForEventAsync(Guid eventId, Guid pollId, CancellationToken cancellationToken);
    Task UpdateAsync(PollEntity pollEntity, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}