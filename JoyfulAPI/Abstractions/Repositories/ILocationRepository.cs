using Joyful.API.Entities;

namespace Joyful.API.Abstractions.Repositories;

public interface ILocationRepository
{
    Task CreateAsync(LocationEntity locationEntity, CancellationToken cancellationToken);
    Task DeleteAsync(LocationEntity locationEntity, CancellationToken cancellationToken);
    Task<IEnumerable<LocationEntity>> ListAsync(CancellationToken cancellationToken);
    Task<LocationEntity> RetrieveAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateAsync(LocationEntity locationEntity, CancellationToken cancellationToken);
}