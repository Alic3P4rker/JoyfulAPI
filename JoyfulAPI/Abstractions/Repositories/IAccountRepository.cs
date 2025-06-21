using Joyful.API.Entities;

namespace Joyful.API.Abstractions.Repositories;

public interface IAccountRepository
{
    Task CreateAsync(AccountEntity accountEntity, CancellationToken cancellationToken);
    Task DeleteAsync(AccountEntity accountEntity, CancellationToken cancellationToken);
    Task<IEnumerable<AccountEntity>> ListAsync(CancellationToken cancellationToken);
    Task<AccountEntity?> RetrieveAsync(Guid Id, CancellationToken cancellationToken);
    Task UpdateAsync(AccountEntity accountEntity, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}