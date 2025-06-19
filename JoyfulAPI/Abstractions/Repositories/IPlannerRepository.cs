using Joyful.API.Entities;

namespace Joyful.API.Abstractions.Repositories;

public interface IPlannerRepository
{
    Task CreateAsync(PlannerEntity plannerEntity, CancellationToken cancellationToken);
    Task DeleteAsync(PlannerEntity plannerEntity, CancellationToken cancellationToken);
    Task<IEnumerable<PlannerEntity>> ListAsync(CancellationToken cancellationToken);
    Task<PlannerEntity> RetrieveAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateAsync(PlannerEntity plannerEntity, CancellationToken cancellationToken);
}