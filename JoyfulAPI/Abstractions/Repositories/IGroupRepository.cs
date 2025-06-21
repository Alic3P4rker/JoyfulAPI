using Joyful.API.Entities;

namespace Joyful.API.Abstractions.Repositories;

public interface IGroupRepository
{
    Task CreateAsync(GroupEntity groupEntity, CancellationToken cancellationToken);
    Task DeleteAsync(GroupEntity groupEntity, CancellationToken cancellationToken);
    Task<IEnumerable<GroupEntity>> ListAsync(CancellationToken cancellationToken);
    Task<GroupEntity?> RetrieveAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateAsync(GroupEntity groupEntity, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}