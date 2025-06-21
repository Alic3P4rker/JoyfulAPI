using Joyful.API.Abstractions.Repositories;
using Joyful.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Joyful.API.Repositories;

internal sealed class LocationRepository : ILocationRepository
{
    private readonly HostDbContext _context;
    public LocationRepository(HostDbContext context)
    {
        _context = context;
    }

    public Task CreateAsync(LocationEntity locationEntity, CancellationToken cancellationToken)
    {
        return _context.Locations.AddAsync(locationEntity, cancellationToken)
                .AsTask();
    }

    public Task DeleteAsync(LocationEntity locationEntity, CancellationToken cancellationToken)
    {
        _context.Locations.Remove(locationEntity);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<LocationEntity>> ListAsync(CancellationToken cancellationToken)
    {
        IEnumerable<LocationEntity> entities = await _context.Locations
            .AsNoTracking()
            .ToArrayAsync(cancellationToken);

        return entities;
    }

    public Task<LocationEntity?> RetrieveAsync(Guid id, CancellationToken cancellationToken)
    {
        return _context.Locations
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id.Equals(id));
    }

    public Task UpdateAsync(LocationEntity locationEntity, CancellationToken cancellationToken)
    {
        EntityEntry<LocationEntity> entry = _context.Locations.Entry(locationEntity);
        entry.State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
       return _context.SaveChangesAsync(cancellationToken);
    }
}