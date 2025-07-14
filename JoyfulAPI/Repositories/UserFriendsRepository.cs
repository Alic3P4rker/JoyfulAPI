using Joyful.API.Abstractions.Repositories;
using Joyful.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Joyful.API.Repositories;

internal sealed class UserFriendsRepository : IUserFriendsRepository
{
    private readonly HostDbContext _context;

    public UserFriendsRepository(HostDbContext context)
    {
        _context = context;
    }

    public Task CreateAsync(UserFriendsEntity UserFriendsEntity, CancellationToken cancellationToken)
    {
        return _context.UserFriends.AddAsync(UserFriendsEntity, cancellationToken)
            .AsTask();
    }

    public Task DeleteAsync(UserFriendsEntity UserFriendsEntity, CancellationToken cancellationToken)
    {
        _context.UserFriends.Remove(UserFriendsEntity);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<UserFriendsEntity>> ListAsync(CancellationToken cancellationToken)
    {
        IEnumerable<UserFriendsEntity> UserFriendsEntity = await _context.UserFriends
            .AsNoTracking()
            .Include(uf => uf.Friend)
            .ToArrayAsync(cancellationToken);

        return UserFriendsEntity;
    }

    public Task<UserFriendsEntity?> RetrieveAsync(Guid userId, Guid friendId, CancellationToken cancellationToken)
    {
        return _context.UserFriends
            .AsNoTracking()
            .Include(uf => uf.Friend)
            .FirstOrDefaultAsync(uf => uf.UserId == userId && uf.FriendId == friendId, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public Task UpdateAsync(UserFriendsEntity UserFriendsEntity, CancellationToken cancellationToken)
    {
        EntityEntry<UserFriendsEntity> entry = _context.UserFriends.Update(UserFriendsEntity);
        entry.State = EntityState.Modified;
        return Task.CompletedTask;
    }
}