using Joyful.API.Entities;

namespace Joyful.API.Abstractions.Repositories;

public interface IUserRepository
{
    Task CreateAsync(UserEntity userEntity, CancellationToken cancellationToken);
    Task DeleteAsync(UserEntity userEntity, CancellationToken cancellationToken);
    Task<IEnumerable<UserEntity>> ListAsync(CancellationToken cancellationToken);
    Task<UserEntity> RetrieveAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateAsync(UserEntity userEntity, CancellationToken cancellationToken);
}