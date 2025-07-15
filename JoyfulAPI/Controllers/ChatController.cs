
using AutoMapper;
using Joyful.API.Abstractions.Repositories;
using Joyful.API.Entities;
using Joyful.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;

[Route("api/[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly IChatRepository _chatRepository;
    private readonly IUserFriendsRepository _userFriendsRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ChatController> _logger;

    public ChatController(
        IUserFriendsRepository userFriendsRepository,
        IChatRepository chatRepository,
        IMapper mapper,
        ILogger<ChatController> logger
    )
    {
        _chatRepository = chatRepository
            ?? throw new ArgumentNullException(nameof(chatRepository));
        _mapper = mapper
            ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
        _userFriendsRepository = userFriendsRepository
            ?? throw new ArgumentNullException(nameof(userFriendsRepository));
    }

    [HttpPost("create-chat")]
    public async Task<IActionResult> CreateChatAsync(ChatCreateDto chatCreateDto, CancellationToken cancellationToken)
    {
        Guid currentUserId = chatCreateDto.creatorId;
        var participantIdsList = chatCreateDto.ParticipantIds.ToList();
        if (!participantIdsList.Contains(chatCreateDto.creatorId))
            participantIdsList.Add(chatCreateDto.creatorId);

        if (participantIdsList.Count() < 2)
            return BadRequest("At least 2 people are required for a chat");

        foreach (Guid participantId in participantIdsList)
        {
            if (participantId == currentUserId)
                continue;

            UserFriendsEntity? userFriendsEntity = await _userFriendsRepository.RetrieveAsync(currentUserId, participantId, cancellationToken);
            if (userFriendsEntity is null)
            {
                _logger.LogWarning("Friendship link doesn't exist between creator {CreatorId} and participant {ParticipantId}", currentUserId, participantId);
                return NotFound($"Friendship doesn't exist between you and participant {participantId}.");
            }
        }

        if (participantIdsList.Count == 2)
        {
            Guid otherParticipantId = participantIdsList.First(id => id != currentUserId);
            ChatEntity? existingChat1 = await _chatRepository.FindOneOnOneChatAsync(currentUserId, otherParticipantId, cancellationToken);
            if (existingChat1 is not null)
            {
                _logger.LogInformation($"Chat already exists between {currentUserId} and {otherParticipantId}");
                return NotFound("Chat already exists");
            }
        }

        if (participantIdsList.Count > 2)
        {
            ChatEntity? exisitingChat2 = await _chatRepository.FindChatbyParticipantsAsync(currentUserId, participantIdsList, cancellationToken);
            if (exisitingChat2 is not null)
            {
                _logger.LogInformation($"Chat already exists for users");
                return NotFound("Chat already exists");
            }
        }

        ChatEntity chatEntity = _mapper.Map<ChatEntity>(chatCreateDto);
        chatEntity.CreatedById = currentUserId;

        foreach (Guid pId in participantIdsList)
        {
            chatEntity.ChatParticipants.Add(new ChatParticipantEntity
            {
                UserId = pId
            });
        }

        ChatDetailsDto chatDetails = _mapper.Map<ChatDetailsDto>(chatEntity);

        await _chatRepository.CreateAsync(chatEntity, cancellationToken);
        await _chatRepository.SaveChangesAsync(cancellationToken);

        return StatusCode(StatusCodes.Status201Created, chatDetails);

    }

    [HttpDelete("delete-chat/{chatId:int}")]
    public async Task<IActionResult> DeleteChatAsync(int chatId, CancellationToken cancellationToken)
    {
        ChatEntity? chatEntity = await _chatRepository.RetrieveChatAsync(chatId, cancellationToken);
        if (chatEntity is null)
        {
            _logger.LogWarning("Chat with ID {ChatId} not found", chatId);
            return NotFound($"Chat with ID {chatId} not found");
        }

        await _chatRepository.DeleteAsync(chatEntity, cancellationToken);
        await _chatRepository.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("retrieve-chat-user")]
    public async Task<ActionResult<ChatDetailsDto>> RetrieveChatAsync(
        [FromQuery] List<Guid> particpantIds,
        CancellationToken cancellationToken
    )
    {

        ChatEntity? chatEntity = null;
        if (particpantIds.Count == 2)
        {
            ChatEntity? existingChat1 = await _chatRepository.FindOneOnOneChatAsync(particpantIds[0], particpantIds[1], cancellationToken);
            if (existingChat1 is null)
            {
                _logger.LogInformation($"Chat doesn't already exist between {particpantIds[0]} and {particpantIds[1]}");
                return NotFound("Chat doesn't exist");
            }
            chatEntity = existingChat1;
        }

        if (particpantIds.Count > 2)
        {
            ChatEntity? exisitingChat2 = await _chatRepository.FindChatbyParticipantsAsync(Guid.Empty, particpantIds, cancellationToken);
            if (exisitingChat2 is null)
            {
                _logger.LogInformation($"Chat doesn't already exist for users");
                return NotFound("Chat doesn't already exist");
            }
            chatEntity = exisitingChat2;
        }

        ChatDetailsDto chatDetails = _mapper.Map<ChatDetailsDto>(chatEntity);
        return Ok(chatDetails);
    }

    [HttpGet("list-chats-user/{currentUserId:guid}")]
    public async Task<ActionResult<IEnumerable<ChatDetailsDto>>> ListChatsAsync(Guid currentUserId, CancellationToken cancellationToken)
    {
        IEnumerable<ChatEntity> chatEntities = await _chatRepository.ListChatsAsync(currentUserId, cancellationToken);
        if (!chatEntities.Any())
        {
            _logger.LogInformation("No chats found for user {UserId}", currentUserId);
            return NotFound("No chats found");
        }

        IEnumerable<ChatDetailsDto> chatDetailsDtos = _mapper.Map<IEnumerable<ChatDetailsDto>>(chatEntities);
        return Ok(chatDetailsDtos);
    }

    [HttpPatch("leave-chat/{chatId:int}")]
    public async Task<IActionResult> LeaveChatAsync(int chatId, Guid userId, CancellationToken cancellationToken)
    {
        ChatEntity? chatEntity = await _chatRepository.RetrieveChatAsync(chatId, cancellationToken);
        if (chatEntity is null)
        {
            _logger.LogWarning("Chat with ID {ChatId} not found", chatId);
            return NotFound($"Chat with ID {chatId} not found");
        }

        Guid currentUserId = userId;
        ChatParticipantEntity? participant = chatEntity.ChatParticipants.FirstOrDefault(p => p.UserId == currentUserId);
        if (participant is null)
        {
            _logger.LogWarning("User {UserId} is not a participant in chat {ChatId}", currentUserId, chatId);
            return NotFound($"You are not a participant in this chat");
        }

        chatEntity.ChatParticipants.Remove(participant);
        await _chatRepository.UpdateAsync(chatEntity, cancellationToken);
        await _chatRepository.SaveChangesAsync(cancellationToken);

        return Ok("You have left the chat successfully");
    }

    [HttpPost("add-to-chat/{chatId:int}/user/{userId:guid}")]
    public async Task<IActionResult> AddUserToChatAsync(int chatId, Guid userId, CancellationToken cancellationToken)
    {
        ChatEntity? chatEntity = await _chatRepository.RetrieveChatAsync(chatId, cancellationToken);
        if (chatEntity is null)
        {
            _logger.LogWarning("Chat with ID {ChatId} not found", chatId);
            return NotFound($"Chat with ID {chatId} not found");
        }

        Guid chatterId = userId;
        if (chatEntity.ChatParticipants.Any(p => p.UserId == chatterId))
        {
            _logger.LogInformation("User {UserId} is already a participant in chat {ChatId}", chatterId, chatId);
            return BadRequest("Person already a participant in this chat");
        }

        chatEntity.ChatParticipants.Add(new ChatParticipantEntity
        {
            UserId = chatterId
        });

        await _chatRepository.UpdateAsync(chatEntity, cancellationToken);
        await _chatRepository.SaveChangesAsync(cancellationToken);

        return Ok("You have added this person to the chat successfully");
    }

    [HttpPatch("update-chat/{chatId:int}")]
    public async Task<IActionResult> UpdateChatAsync(int chatId, ChatUpdateDto chatUpdateDto, CancellationToken cancellationToken)
    {
        ChatEntity? chatEntity = await _chatRepository.RetrieveChatAsync(chatId, cancellationToken);
        if (chatEntity is null)
        {
            _logger.LogWarning("Chat with ID {ChatId} not found", chatId);
            return NotFound($"Chat with ID {chatId} not found");
        }

        chatEntity.Name = chatUpdateDto.Name;
        await _chatRepository.UpdateAsync(chatEntity, cancellationToken);
        await _chatRepository.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}