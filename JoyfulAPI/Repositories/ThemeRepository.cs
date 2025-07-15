using Joyful.API.Abstractions.Repositories;
using Joyful.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Joyful.API.Repositories;

public sealed class ThemeRepository : IThemeRepository
{
    private readonly HostDbContext _context;

    public ThemeRepository(HostDbContext context)
    {
        _context = context;
    }

    public Task CreateAsync(ThemeEntity themeEntity, CancellationToken cancellationToken)
    {
        return _context.Themes.AddAsync(themeEntity, cancellationToken)
                .AsTask();
    }

    public Task DeleteAsync(ThemeEntity themeEntity, CancellationToken cancellationToken)
    {
        _context.Themes.Remove(themeEntity);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<ThemeEntity>> ListAsync(CancellationToken cancellationToken)
    {
        IEnumerable<ThemeEntity> entities = await _context.Themes
            .AsNoTracking()
            .ToArrayAsync(cancellationToken);

        return entities;
    }

    public Task<ThemeEntity?> RetrieveAsync(Guid id, CancellationToken cancellationToken)
    {
        return _context.Themes
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id.Equals(id));
    }

    public Task UpdateAsync(ThemeEntity themeEntity, CancellationToken cancellationToken)
    {
        EntityEntry<ThemeEntity> entry = _context.Themes.Entry(themeEntity);
        entry.State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}