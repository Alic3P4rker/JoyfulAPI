using Joyful.API.Abstractions.Repositories;
using Joyful.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Joyful.API.Repositories;

public sealed class VoteRepository : IVoteRepository
{
    private readonly HostDbContext _context;

    public VoteRepository(HostDbContext context)
    {
        _context = context;
    }

    public Task CreateAsync(VoteEntity voteEntity, CancellationToken cancellationToken)
    {
        return _context.Votes.AddAsync(voteEntity, cancellationToken)
            .AsTask();
    }

    public Task DeleteAsync(VoteEntity voteEntity, CancellationToken cancellationToken)
    {
        _context.Votes.Remove(voteEntity);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<VoteEntity>> ListAsync(CancellationToken cancellationToken)
    {
        IEnumerable<VoteEntity> entities = await _context.Votes
            .AsNoTracking()
            .ToArrayAsync();

        return entities;
    }

    public async Task<IEnumerable<VoteEntity>> ListVotesForPollAsync(Guid id, CancellationToken cancellationToken)
    {
        IEnumerable<VoteEntity> entities = await _context.Votes
            .AsNoTracking()
            .Where(v => v.PollId.Equals(id))
            .ToArrayAsync();

        return entities;
    }

    public Task<VoteEntity?> RetrieveAsync(Guid id, CancellationToken cancellationToken)
    {
        return _context.Votes
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id.Equals(id), cancellationToken);
    }

    public Task UpdateAsync(VoteEntity voteEntity, CancellationToken cancellationToken)
    {
        EntityEntry<VoteEntity> entry = _context.Votes.Entry(voteEntity);
        entry.State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}