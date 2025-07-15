using AutoMapper;
using Joyful.API.Abstractions.Repositories;
using Joyful.API.Entities;
using Joyful.API.Enums;
using Joyful.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Joyful.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MessagesController : ControllerBase
{
    private readonly IMessageRepository _messageRepository;
    private readonly IChatRepository _chatRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<MessagesController> _logger;

    public MessagesController(
        IChatRepository chatRepository,
        IMessageRepository messageRepository,
        IMapper mapper,
        ILogger<MessagesController> logger)
    {
        _chatRepository = chatRepository
            ?? throw new ArgumentNullException(nameof(chatRepository));
        _messageRepository = messageRepository
            ?? throw new ArgumentNullException(nameof(messageRepository));
        _mapper = mapper
            ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost("send-message")]
    public async Task<IActionResult> SendMessageAsync([FromBody] MessageCreateDto messageDto, CancellationToken cancellationToken)
    {
        ChatEntity? chat = await _chatRepository.RetrieveChatAsync(messageDto.ChatId, cancellationToken);
        if (chat == null)
        {
            _logger.LogWarning("Chat with ID {ChatId} not found.", messageDto.ChatId);
            return NotFound($"Chat with ID {messageDto.ChatId} not found.");
        }

        if (chat.ChatParticipants.All(p => p.UserId != messageDto.SenderId))
        {
            _logger.LogWarning("User with ID {SenderId} is not a participant of chat with ID {ChatId}.", messageDto.SenderId, messageDto.ChatId);
            return BadRequest($"User with ID {messageDto.SenderId} is not a participant of chat with ID {messageDto.ChatId}.");
        }

        MessageEntity message = _mapper.Map<MessageEntity>(messageDto);
        message.SentAt = DateTime.UtcNow;
        message.Status = MessageStatus.Sent;

        await _messageRepository.CreateAsync(message, cancellationToken);
        await _messageRepository.SaveChangesAsync(cancellationToken);

        MessageDetailsDto messageDetailsDto = _mapper.Map<MessageDetailsDto>(message);
        _logger.LogInformation("Message sent successfully to chat with ID {ChatId}.", messageDto.ChatId);

        return StatusCode(StatusCodes.Status201Created, messageDetailsDto);
    }

    [HttpGet("retrieve-messages/{chatId:int}")]
    public async Task<IActionResult> GetMessagesByChatIdAsync(int chatId, CancellationToken cancellationToken)
    {
        IEnumerable<MessageEntity> messages = await _messageRepository.GetMessagesByChatIdAsync(chatId, cancellationToken);
        if (messages == null || !messages.Any())
        {
            _logger.LogInformation("No messages found for chat with ID {ChatId}.", chatId);
            return NotFound($"No messages found for chat with ID {chatId}.");
        }

        IEnumerable<MessageDetailsDto> messageDtos = _mapper.Map<IEnumerable<MessageDetailsDto>>(messages);
        _logger.LogInformation("Retrieved {MessageCount} messages for chat with ID {ChatId}.", messageDtos.Count(), chatId);
        return Ok(messageDtos);
    }

    [HttpGet("search-messages/{chatId:int}")]
    public async Task<IActionResult> SearchMessagesAsync(int chatId, [FromQuery] string searchTerm, CancellationToken cancellationToken)
    {
#warning Possibility add a fail safe incase the message are too much or search term is too long or short
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            _logger.LogWarning("Search term cannot be empty.");
            return BadRequest("Search term cannot be empty.");
        }
        IEnumerable<MessageEntity> messages = await _messageRepository.SearchMessagesAsync(chatId, searchTerm, cancellationToken);
        if (messages == null || !messages.Any())
        {
            _logger.LogInformation("No messages found for chat with ID {ChatId} containing search term '{SearchTerm}'.", chatId, searchTerm);
            return NotFound($"No messages found for chat with ID {chatId} containing search term '{searchTerm}'.");
        }

        IEnumerable<MessageDetailsDto> messageDtos = _mapper.Map<IEnumerable<MessageDetailsDto>>(messages);
        _logger.LogInformation("Found {MessageCount} messages for chat with ID {ChatId} containing search term '{SearchTerm}'.", messageDtos.Count(), chatId, searchTerm);
        return Ok(messageDtos);
    }

    [HttpPatch("update-message/{messageId:int}")]
    public async Task<IActionResult> UpdateMessageAsync(int messageId, [FromBody] MessageUpdateDto messageDto, CancellationToken cancellationToken)
    {
        MessageEntity? existingMessage = await _messageRepository.RetrieveMessageByIdAsync(messageId, cancellationToken);
        if (existingMessage == null)
        {
            _logger.LogWarning("Message with ID {MessageId} not found.", messageId);
            return NotFound($"Message with ID {messageId} not found.");
        }

        existingMessage.Content = messageDto.Content;
        existingMessage.SentAt = DateTime.UtcNow;

        await _messageRepository.UpdateAsync(existingMessage, cancellationToken);
        await _messageRepository.SaveChangesAsync(cancellationToken);

        MessageDetailsDto updatedMessageDto = _mapper.Map<MessageDetailsDto>(existingMessage);
        _logger.LogInformation("Message with ID {MessageId} updated successfully.", messageId);

        return Ok(updatedMessageDto);
    }

    [HttpDelete("delete-message/{messageId:int}")]
    public async Task<IActionResult> DeleteMessageAsync(int messageId, CancellationToken cancellationToken)
    {
        MessageEntity? existingMessage = await _messageRepository.RetrieveMessageByIdAsync(messageId, cancellationToken);
        if (existingMessage == null)
        {
            _logger.LogWarning("Message with ID {MessageId} not found.", messageId);
            return NotFound($"Message with ID {messageId} not found.");
        }

        await _messageRepository.DeleteAsync(existingMessage, cancellationToken);
        await _messageRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Message with ID {MessageId} deleted successfully.", messageId);
        return NoContent();
    }
}