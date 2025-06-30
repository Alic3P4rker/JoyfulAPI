using Joyful.API.Entities;

namespace Joyful.API.Abstractions.Repositories;

public interface IThemeRepository
{
    Task CreateAsync(ThemeEntity themeEntity, CancellationToken cancellationToken);
    Task DeleteAsync(ThemeEntity themeEntity, CancellationToken cancellationToken);
    Task<ThemeEntity?> RetrieveAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<ThemeEntity>> ListAsync(CancellationToken cancellationToken);
    Task UpdateAsync(ThemeEntity themeEntity, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}