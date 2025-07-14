
using AutoMapper;
using Joyful.API.Abstractions.Repositories;
using Joyful.API.Entities;
using Joyful.API.Models;
using Microsoft.AspNetCore.Mvc;

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
}