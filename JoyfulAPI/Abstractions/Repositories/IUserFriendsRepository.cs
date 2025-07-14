using Joyful.API.Entities;

namespace Joyful.API.Abstractions.Repositories;

public interface IUserFriendsRepository
{
    Task CreateAsync(UserFriendsEntity UserFriendsEntity, CancellationToken cancellationToken);
    Task DeleteAsync(UserFriendsEntity UserFriendsEntity, CancellationToken cancellationToken);
    Task<IEnumerable<UserFriendsEntity>> ListAsync(CancellationToken cancellationToken);
    Task<UserFriendsEntity?> RetrieveAsync(Guid userId, Guid friendId, CancellationToken cancellationToken);
    Task UpdateAsync(UserFriendsEntity UserFriendsEntity, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}