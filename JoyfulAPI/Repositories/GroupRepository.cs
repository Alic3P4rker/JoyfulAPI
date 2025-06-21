using Joyful.API.Abstractions.Repositories;
using Joyful.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Joyful.API.Repositories;

internal sealed class GroupRepository : IGroupRepository
{
    private readonly HostDbContext _context;
    public GroupRepository(HostDbContext context)
    {
        _context = context;
    }
    public Task CreateAsync(GroupEntity groupEntity, CancellationToken cancellationToken)
    {
        return _context.Groups.AddAsync(groupEntity, cancellationToken)
                .AsTask();

    }

    public Task DeleteAsync(GroupEntity groupEntity, CancellationToken cancellationToken)
    {
        _context.Groups.Remove(groupEntity);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<GroupEntity>> ListAsync(CancellationToken cancellationToken)
    {
        IEnumerable<GroupEntity> entities = await _context.Groups
            .AsNoTracking()
            .ToArrayAsync(cancellationToken);

        return entities;
    }

    public Task<GroupEntity?> RetrieveAsync(Guid id, CancellationToken cancellationToken)
    {
        return _context.Groups
                    .AsNoTracking()
                    .FirstOrDefaultAsync(g => g.Id.Equals(id), cancellationToken);
    }

    public Task UpdateAsync(GroupEntity groupEntity, CancellationToken cancellationToken)
    {
        EntityEntry<GroupEntity> entry = _context.Groups.Entry(groupEntity);
        entry.State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
       return _context.SaveChangesAsync(cancellationToken);
    }
}