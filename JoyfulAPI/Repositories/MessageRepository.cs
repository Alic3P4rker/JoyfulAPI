using Joyful.API.Abstractions.Repositories;
using Joyful.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Joyful.API.Repositories;

internal sealed class MessageRepository : IMessageRepository
{
    private readonly HostDbContext _context;
    public MessageRepository(HostDbContext context)
    {
        _context = context;
    }
    public Task CreateAsync(MessageEntity message, CancellationToken cancellationToken)
    {
        return _context.Messages.AddAsync(message, cancellationToken)
            .AsTask();
    }

    public Task DeleteAsync(MessageEntity message, CancellationToken cancellationToken)
    {
        _context.Messages.Remove(message);
        return Task.CompletedTask;
    }

    public Task<MessageEntity?> RetrieveMessageByIdAsync(int messageId, CancellationToken cancellationToken)
    {
        return _context.Messages
            .FirstOrDefaultAsync(m => m.Id.Equals(messageId), cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<MessageEntity>> SearchMessagesAsync(int chatId, string searchTerm, CancellationToken cancellationToken)
    {
        IEnumerable<MessageEntity> messages = await _context.Messages
            .AsNoTracking()
            .Where(m => m.ChatId == chatId && m.Content.Contains(searchTerm))
            .ToArrayAsync(cancellationToken);
        
        return messages;
    }

    public Task<IEnumerable<MessageEntity>> GetMessagesByChatIdAsync(int chatId, CancellationToken cancellationToken)
    {
        IEnumerable<MessageEntity> messages = _context.Messages
            .AsNoTracking()
            .Where(m => m.ChatId == chatId)
            .OrderBy(m => m.SentAt);

        return Task.FromResult(messages);
    }

    public Task UpdateAsync(MessageEntity message, CancellationToken cancellationToken)
    {
        EntityEntry<MessageEntity> entry = _context.Messages.Update(message);
        entry.State = EntityState.Modified;
        return Task.CompletedTask;
    }
}