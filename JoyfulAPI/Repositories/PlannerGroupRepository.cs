using Joyful.API.Abstractions.Repositories;
using Joyful.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Joyful.API.Repositories;

public sealed class PlannerGroupRepository : IPlannerGroupRepository
{
    private readonly HostDbContext _context;

    public PlannerGroupRepository(HostDbContext context)
    {
        _context = context;
    }
    public Task CreateAsync(PlannerGroupEntity plannerGroupEntity, CancellationToken cancellationToken)
    {
        _context.PlannerGroups.AddAsync(plannerGroupEntity, cancellationToken)
                .AsTask();

        return _context.SaveChangesAsync(cancellationToken);
    }

    public Task DeleteAsync(PlannerGroupEntity plannerGroupEntity, CancellationToken cancellationToken)
    {
        _context.PlannerGroups.Remove(plannerGroupEntity);

        return _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<GroupEntity>> ListGroupsByPlannerId(Guid id, CancellationToken cancellationToken)
    {
        IEnumerable<GroupEntity> entities = await _context.PlannerGroups
            .AsNoTracking()
            .Where(p => p.PlannerId.Equals(id))
            .Select(p => p.Group)
            .ToArrayAsync(cancellationToken);

        return entities;
    }

    public async Task<GroupEntity?> RetrieveGroup(Guid plannerId, Guid groupId, CancellationToken cancellationToken)
    {
        GroupEntity? groupEntity = await _context.PlannerGroups
            .AsNoTracking()
            .Where(p => p.PlannerId.Equals(plannerId) && p.GroupId.Equals(groupId))
            .Select(p => p.Group)
            .FirstOrDefaultAsync(cancellationToken);

        return groupEntity;
    }
}