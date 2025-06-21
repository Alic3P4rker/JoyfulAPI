using Joyful.API.Abstractions.Repositories;
using Joyful.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Joyful.API.Repositories;

internal sealed class UserRepository : IUserRepository
{
    private readonly HostDbContext _context;

    public UserRepository(HostDbContext context)
    {
        _context = context;
    }

    public Task CreateAsync(UserEntity userEntity, CancellationToken cancellationToken)
    {
        return _context.User.AddAsync(userEntity, cancellationToken)
                .AsTask();
    }

    public Task DeleteAsync(UserEntity userEntity, CancellationToken cancellationToken)
    {
        _context.User.Remove(userEntity);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<UserEntity>> ListAsync(CancellationToken cancellationToken)
    {
        IEnumerable<UserEntity> entities = await _context.User
            .AsNoTracking()
            .ToArrayAsync();

        return entities;
    }

    public Task<UserEntity?> RetrieveByEmailAsync(string emailAddress, CancellationToken cancellationToken)
    {
        return _context.User
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.EmailAddress.Equals(emailAddress), cancellationToken);
    }

    public Task<UserEntity?> RetrieveByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _context.User
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id.Equals(id), cancellationToken);
    }

    public Task UpdateAsync(UserEntity userEntity, CancellationToken cancellationToken)
    {
        EntityEntry<UserEntity> entry = _context.User.Entry(userEntity);
        entry.State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

}