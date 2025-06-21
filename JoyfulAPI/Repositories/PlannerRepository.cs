using Joyful.API.Abstractions.Repositories;
using Joyful.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Joyful.API.Repositories;

internal sealed class PlannerRepository : IPlannerRepository
{
    private readonly HostDbContext _context;

    public PlannerRepository(HostDbContext context)
    {
        _context = context;
    }

    public Task CreateAsync(PlannerEntity plannerEntity, CancellationToken cancellationToken)
    {
        return _context.Planners.AddAsync(plannerEntity, cancellationToken)
                .AsTask();
    }

    public Task DeleteAsync(PlannerEntity plannerEntity, CancellationToken cancellationToken)
    {
        _context.Planners.Remove(plannerEntity);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<PlannerEntity>> ListAsync(CancellationToken cancellationToken)
    {
        IEnumerable<PlannerEntity> entities = await _context.Planners
            .AsNoTracking()
            .ToArrayAsync();

        return entities;
    }

    public Task<PlannerEntity?> RetrieveAsync(Guid id, CancellationToken cancellationToken)
    {
        return _context.Planners
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id.Equals(id));
    }

    public Task UpdateAsync(PlannerEntity plannerEntity, CancellationToken cancellationToken)
    {
        EntityEntry<PlannerEntity> entry = _context.Planners.Entry(plannerEntity);
        entry.State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}