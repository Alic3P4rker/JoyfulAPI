using Joyful.API.Abstractions.Repositories;
using Joyful.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Joyful.API.Repositories;

internal sealed class AccountRepository : IAccountRepository
{
    private readonly HostDbContext _context;
    public AccountRepository(HostDbContext context)
    {
        _context = context;
    }
    public Task CreateAsync(AccountEntity accountEntity, CancellationToken cancellationToken)
    {
        return _context.Accounts.AddAsync(accountEntity, cancellationToken)
                .AsTask();
    }

    public Task DeleteAsync(AccountEntity accountEntity, CancellationToken cancellationToken)
    {
        _context.Accounts.Remove(accountEntity);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<AccountEntity>> ListAsync(CancellationToken cancellationToken)
    {
        IEnumerable<AccountEntity> entities = await _context.Accounts
            .AsNoTracking()
            .ToArrayAsync(cancellationToken);

        return entities;
    }

    public Task<AccountEntity?> RetrieveAsync(Guid Id, CancellationToken cancellationToken)
    {
        return _context.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id.Equals(Id), cancellationToken);
    }

    public Task UpdateAsync(AccountEntity accountEntity, CancellationToken cancellationToken)
    {
        EntityEntry<AccountEntity> entry = _context.Accounts.Entry(accountEntity);
        entry.State = EntityState.Modified;
        return Task.CompletedTask;
    }
    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}