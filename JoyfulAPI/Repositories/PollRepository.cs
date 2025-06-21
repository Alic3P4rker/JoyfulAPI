using Joyful.API.Abstractions.Repositories;
using Joyful.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Joyful.API.Repositories;

internal sealed class PollRepository : IPollRepository
{
    private readonly HostDbContext _context;
    public PollRepository(HostDbContext context)
    {
        _context = context;
    }

    public Task CreateAsync(PollEntity pollEntity, CancellationToken cancellationToken)
    {
        return _context.Polls.AddAsync(pollEntity, cancellationToken)
            .AsTask();
    }

    public Task DeleteAsync(PollEntity pollEntity, CancellationToken cancellationToken)
    {
        _context.Polls.Remove(pollEntity);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<PollEntity>> ListAsync(CancellationToken cancellationToken)
    {
        IEnumerable<PollEntity> entities = await _context.Polls
            .AsNoTracking()
            .ToArrayAsync();

        return entities;
    }

    public Task<PollEntity?> RetrieveAsync(Guid id, CancellationToken cancellationToken)
    {
        return _context.Polls
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);
    }

    public Task UpdateAsync(PollEntity pollEntity, CancellationToken cancellationToken)
    {
        EntityEntry<PollEntity> entry = _context.Polls.Entry(pollEntity);
        entry.State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}