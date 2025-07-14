using Joyful.API.Abstractions.Repositories;
using Joyful.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Joyful.API.Repositories;

internal sealed class ChatRepository : IChatRepository
{
    private readonly HostDbContext _context;
    public ChatRepository(HostDbContext context)
    {
        _context = context
            ?? throw new ArgumentNullException(nameof(context));
    }

    public Task CreateAsync(ChatEntity chat, CancellationToken cancellationToken)
    {
        return _context.Chats.AddAsync(chat, cancellationToken)
            .AsTask();
    }

    public Task DeleteAsync(ChatEntity chat, CancellationToken cancellationToken)
    {
        _context.Chats.Remove(chat);
        return Task.CompletedTask;
    }

    public Task<ChatEntity?> FindChatbyParticipantsAsync(Guid creatorId, List<Guid> participantIds, CancellationToken cancellationToken)
    {
        var sortedParticipantIds = participantIds
            .OrderBy(id => id)
            .ToList();
        int participantsCount = sortedParticipantIds.Count;

        return _context.Chats
            .AsNoTracking()
            .Include(c => c.ChatParticipants)
            .Where(c => 
                    c.ChatParticipants.Count == participantsCount &&
                    c.ChatParticipants.All(p => sortedParticipantIds.Contains(p.UserId)))
            .FirstOrDefaultAsync(cancellationToken);
    }


    public Task<ChatEntity?> FindOneOnOneChatAsync(Guid userId1, Guid userId2, CancellationToken cancellationToken)
    {
        return _context.Chats
            .AsNoTracking()
            .Include(c => c.ChatParticipants)
            .FirstOrDefaultAsync(c => c.ChatParticipants.Count == 2 &&
                                      c.ChatParticipants.Any(p => p.UserId == userId1) &&
                                      c.ChatParticipants.Any(p => p.UserId == userId2), cancellationToken);
    }

    public async Task<IEnumerable<ChatEntity>> ListChatsAsync(Guid userid, CancellationToken cancellationToken)
    {
        IEnumerable<ChatEntity> chats = await _context.Chats
            .AsNoTracking()
            .Include(c => c.Messages)
            .Include(c => c.ChatParticipants)
            .Where(c => c.CreatedById.Equals(userid) || 
                        c.ChatParticipants.Any(p => p.UserId.Equals(userid)))
            .ToArrayAsync(cancellationToken);

        return chats;
    }

    public Task<ChatEntity?> RetrieveChatAsync(int chatId, CancellationToken cancellationToken)
    {
        return _context.Chats
            .AsNoTracking()
            .Include(c => c.Messages)
            .Include(c => c.ChatParticipants)
            .FirstOrDefaultAsync(c => c.Id.Equals(chatId), cancellationToken);

    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public Task UpdateAsync(ChatEntity chat, CancellationToken cancellationToken)
    {
        EntityEntry<ChatEntity> entry = _context.Chats.Entry(chat);
        entry.State = EntityState.Modified;
        return Task.CompletedTask;
    }
}