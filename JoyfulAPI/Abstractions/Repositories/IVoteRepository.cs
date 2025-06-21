using Joyful.API.Entities;

namespace Joyful.API.Abstractions.Repositories;

public interface IVoteRepository
{
    Task CreateAsync(VoteEntity voteEntity, CancellationToken cancellationToken);
    Task DeleteAsync(VoteEntity voteEntity, CancellationToken cancellationToken);
    Task<IEnumerable<VoteEntity>> ListAsync(CancellationToken cancellationToken);
    Task<VoteEntity?> RetrieveAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateAsync(VoteEntity voteEntity, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}