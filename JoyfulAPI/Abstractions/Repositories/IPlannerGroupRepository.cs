using Joyful.API.Entities;

namespace Joyful.API.Abstractions.Repositories;

public interface IPlannerGroupRepository
{
    Task CreateAsync(PlannerGroupEntity plannerGroupEntity, CancellationToken cancellationToken);
    Task DeleteAsync(PlannerGroupEntity plannerGroupEntity, CancellationToken cancellationToken);
    Task<IEnumerable<GroupEntity>> ListGroupsByPlannerId(Guid id, CancellationToken cancellationToken);
    Task<GroupEntity?> RetrieveGroup(Guid plannerId, Guid groupId, CancellationToken cancellationToken);
}