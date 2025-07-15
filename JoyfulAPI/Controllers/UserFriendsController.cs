using AutoMapper;
using Joyful.API.Abstractions.Repositories;
using Joyful.API.Entities;
using Joyful.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Joyful.API.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class UserFriendsController : ControllerBase
{
    private readonly IUserFriendsRepository _userFriendsRepository;
    private readonly IPlannerRepository _plannerRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserFriendsController> _logger;

    public UserFriendsController(
        IUserFriendsRepository userFriendsRepository,
        IPlannerRepository plannerRepository,
        IMapper mapper,
        ILogger<UserFriendsController> logger
    )
    {
        _userFriendsRepository = userFriendsRepository
            ?? throw new ArgumentNullException(nameof(userFriendsRepository));
        _plannerRepository = plannerRepository
            ?? throw new ArgumentNullException(nameof(plannerRepository));
        _mapper = mapper
            ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost("add-new-friend")]
    public async Task<IActionResult> AddNewFriendAsync([FromBody] UserFriendsCreateDto userDto, CancellationToken cancellationToken)
    {
        UserFriendsEntity? UserFriendsEntity = await _userFriendsRepository.RetrieveAsync(userDto.UserId, userDto.FriendId, cancellationToken);
        if (UserFriendsEntity is not null)
        {
            _logger.LogInformation("Friend with ID found for user with ID", userDto.FriendId, userDto.UserId);
            return Conflict($"Friend with ID {userDto.FriendId} found for user with ID {userDto.UserId}.");
        }

        UserFriendsEntity userFriendsEntity = _mapper.Map<UserFriendsEntity>(userDto);
        await _userFriendsRepository.CreateAsync(userFriendsEntity, cancellationToken);
        await _userFriendsRepository.SaveChangesAsync(cancellationToken);

        PlannerEntity? plannerEntity = await _plannerRepository.RetrieveAsync(userFriendsEntity.FriendId, cancellationToken);
        if (plannerEntity is null)
        {
            _logger.LogInformation("Planner with ID {FriendId} not found for user with ID {UserId}.", userFriendsEntity.FriendId, userFriendsEntity.UserId);
            return NotFound($"Planner with ID {userFriendsEntity.FriendId} not found for user with ID {userFriendsEntity.UserId}.");
        }

        PlannerDto plannerInformation = _mapper.Map<PlannerDto>(plannerEntity);
        return StatusCode(StatusCodes.Status201Created, plannerInformation);
    }

    [HttpDelete("delete-friend/{userId}/{friendId}")]
    public async Task<IActionResult> DeleteFriendAsync(Guid userId, Guid friendId, CancellationToken cancellationToken)
    {
        UserFriendsEntity? UserFriendsEntity = await _userFriendsRepository.RetrieveAsync(userId, friendId, cancellationToken);
        if (UserFriendsEntity is null)
        {
            _logger.LogInformation("Friend with ID {FriendId} not found for user with ID {UserId}.", friendId, userId);
            return NotFound($"Friend with ID {friendId} not found for user with ID {userId}.");
        }

        await _userFriendsRepository.DeleteAsync(UserFriendsEntity, cancellationToken);
        await _userFriendsRepository.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpGet("get-friend/{userId}/{friendId}")]
    public async Task<IActionResult> GetFriendAsync(Guid userId, Guid friendId, CancellationToken cancellationToken)
    {
        UserFriendsEntity? userFriendsEntity = await _userFriendsRepository.RetrieveAsync(userId, friendId, cancellationToken);
        if (userFriendsEntity is null)
        {
            _logger.LogInformation("Friend with ID {FriendId} not found for user with ID {UserId}.", friendId, userId);
            return NotFound($"Friend with ID {friendId} not found for user with ID {userId}.");
        }

        PlannerEntity? plannerEntity = await _plannerRepository.RetrieveAsync(userFriendsEntity.FriendId, cancellationToken);
        if (plannerEntity is null)
        {
            _logger.LogInformation("Planner with ID {FriendId} not found for user with ID {UserId}.", userFriendsEntity.FriendId, userFriendsEntity.UserId);
            return NotFound($"Planner with ID {userFriendsEntity.FriendId} not found for user with ID {userFriendsEntity.UserId}.");
        }

        PlannerDto plannerInformation = _mapper.Map<PlannerDto>(plannerEntity);
        return Ok(plannerInformation);
    }

    [HttpPut("update-friend/{userId}/{friendId}")]
    public async Task<IActionResult> UpdateFriendAsync(Guid userId, Guid friendId, [FromBody] UserFriendsDetailsDto userDto, CancellationToken cancellationToken)
    {
        UserFriendsEntity? userFriendsEntity = await _userFriendsRepository.RetrieveAsync(userId, friendId, cancellationToken);
        if (userFriendsEntity is null)
        {
            _logger.LogInformation("Friend with ID {FriendId} not found for user with ID {UserId}.", friendId, userId);
            return NotFound($"Friend with ID {friendId} not found for user with ID {userId}.");
        }

        _mapper.Map(userDto, userFriendsEntity);
        await _userFriendsRepository.UpdateAsync(userFriendsEntity, cancellationToken);
        await _userFriendsRepository.SaveChangesAsync(cancellationToken);

        UserFriendsDetailsDto UserFriendsEntityDetails = _mapper.Map<UserFriendsDetailsDto>(userFriendsEntity);
        return Ok(UserFriendsEntityDetails);
    }

    [HttpGet("list-all-friends-for-user/{userId:guid}")]
    public async Task<IActionResult> GetAllFriendsForUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        IEnumerable<UserFriendsEntity> userFriendsEntities = await _userFriendsRepository.ListAsync(userId, cancellationToken);
        if (!userFriendsEntities.Any())
        {
            _logger.LogInformation("No friends found.");
            return NotFound("No friends found.");
        }

        IEnumerable<PlannerEntity> plannerEntities = new List<PlannerEntity>();
        foreach (UserFriendsEntity userFriends in userFriendsEntities)
        {
            PlannerEntity? plannerEntity = await _plannerRepository.RetrieveAsync(userFriends.FriendId, cancellationToken);
            if (plannerEntity is null)
            {
                _logger.LogInformation("Planner with ID {FriendId} not found for user with ID {UserId}.", userFriends.FriendId, userFriends.UserId);
                return NotFound($"Planner with ID {userFriends.FriendId} not found for user with ID {userFriends.UserId}.");
            }
            plannerEntities = plannerEntities.Append(plannerEntity);
        }

        IEnumerable<PlannerDto> plannersInformation = _mapper.Map<IEnumerable<PlannerDto>>(plannerEntities);
        return Ok(plannersInformation);
    }

    [HttpGet("list-all-potential-friends/{userId:guid}")]
    public async Task<IActionResult> ListAllPotentialFriendsAsync(Guid userId, CancellationToken cancellationToken)
    {
        IEnumerable<PlannerEntity> plannerEntities = await _plannerRepository.ListAsync(cancellationToken);
        if (!plannerEntities.Any())
        {
            _logger.LogInformation("No potential friends found.");
            return NotFound("No potential friends found.");
        }

        IEnumerable<PlannerDto> potentialFriends = new List<PlannerDto>();

        foreach (PlannerEntity plannerEntity in plannerEntities)
        {
            if (plannerEntity.Id == userId)
            {
                continue;
            }

            UserFriendsEntity? userFriendsEntity = await _userFriendsRepository.RetrieveAsync(userId, plannerEntity.Id, cancellationToken);
            if (userFriendsEntity is null)
            {
                potentialFriends = potentialFriends.Append(_mapper.Map<PlannerDto>(plannerEntity));
            }
        }

        return Ok(potentialFriends);
    }
}