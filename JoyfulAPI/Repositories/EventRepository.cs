using Joyful.API.Abstractions.Repositories;
using Joyful.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Joyful.API.Repositories;

internal sealed class EventRepository : IEventRepository
{
    private readonly HostDbContext _context;
    public EventRepository(HostDbContext context)
    {
        _context = context;
    }
    public Task CreateAsync(EventEntity eventEntity, CancellationToken cancellationToken)
    {
        return _context.Events.AddAsync(eventEntity, cancellationToken)
            .AsTask();
    }

    public Task DeleteAsync(EventEntity eventEntity, CancellationToken cancellationToken)
    {
        _context.Events.Remove(eventEntity);
        return Task.CompletedTask;

    }

    public async Task<IEnumerable<EventEntity>> ListAsync(CancellationToken cancellationToken)
    {
        IEnumerable<EventEntity> entities = await _context.Events
            .AsNoTracking()
            .ToArrayAsync(cancellationToken);

        return entities;
    }

    public Task<EventEntity?> RetrieveAsync(Guid id, CancellationToken cancellationToken)
    {
        return _context.Events
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
    }

    public Task UpdateAsync(EventEntity eventEntity, CancellationToken cancellationToken)
    {
        EntityEntry<EventEntity> entry = _context.Events.Entry(eventEntity);
        entry.State = EntityState.Modified;
        return Task.CompletedTask;
    }
    
    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}