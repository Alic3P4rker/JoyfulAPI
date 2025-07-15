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
public class EventController : ControllerBase
{
    private readonly IEventRepository _eventRepository;
    private readonly IPlannerGroupRepository _plannerGroupRepository;
    private readonly IPlannerRepository _plannerRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<EventController> _logger;

    public EventController(
        IEventRepository eventRepository,
        IPlannerRepository plannerRepository,
        IPlannerGroupRepository plannerGroupRepository,
        IMapper mapper,
        ILogger<EventController> logger)
    {
        _plannerRepository = plannerRepository;
        _plannerGroupRepository = plannerGroupRepository;
        _eventRepository = eventRepository;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateEventAsync([FromBody] EventCreateDto eventDto, CancellationToken cancellationToken)
    {
        //Step 1: Verify planner
        PlannerEntity? plannerEntity = await _plannerRepository.RetrieveAsync(eventDto.createdPlannerId, cancellationToken);
        if (plannerEntity is null)
        {
            _logger.LogInformation($"Planner by this id: {eventDto.createdPlannerId} does not existed");
            return NotFound("Planner doesn't exist");
        }

        //Step 2: Verify planner is linked to group
        GroupEntity? groupEntity = await _plannerGroupRepository.RetrieveGroup(eventDto.createdPlannerId, eventDto.groupId, cancellationToken);
        if (groupEntity is null)
        {
            _logger.LogInformation("Planner is not linked this to group or group doesn't exist");
            return NotFound("Planner is not linked this to group or group doesn't exist");
        }

        //Step 3: Create Event
        EventEntity eventEntity = _mapper.Map<EventEntity>(eventDto);
        eventEntity.CreatedAt = DateTimeOffset.Now;
        eventEntity.UpdatedAt = DateTimeOffset.Now;
        await _eventRepository.CreateAsync(eventEntity, cancellationToken);
        await _eventRepository.SaveChangesAsync(cancellationToken);

        EventDetailsDto eventDetailsDto = _mapper.Map<EventDetailsDto>(eventEntity);
        return StatusCode(StatusCodes.Status201Created, eventDetailsDto);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteEventAsync(Guid id, CancellationToken cancellationToken)
    {
        EventEntity? eventEntity = await _eventRepository.RetrieveAsync(id, cancellationToken);
        if (eventEntity is null)
        {
            _logger.LogInformation($"Event doesn't exist for this id: {id}");
            return NotFound("Event doesn't exist");
        }

        await _eventRepository.DeleteAsync(eventEntity, cancellationToken);
        await _eventRepository.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EventDetailsDto>> RetrieveEventAsync(Guid id, CancellationToken cancellationToken)
    {
        EventEntity? eventEntity = await _eventRepository.RetrieveAsync(id, cancellationToken);
        if (eventEntity is null)
        {
            _logger.LogInformation($"Event doesn't exist for this id: {id}");
            return NotFound("Event doesn't exist");
        }

        EventDetailsDto eventDetails = _mapper.Map<EventDetailsDto>(eventEntity);
        return Ok(eventDetails);
    }

    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<EventDetailsDto>>> ListEventsAsync(CancellationToken cancellationToken)
    {
        IEnumerable<EventEntity> eventEntities = await _eventRepository.ListAsync(cancellationToken);
        IEnumerable<EventDetailsDto> eventDetails = _mapper.Map<IEnumerable<EventDetailsDto>>(eventEntities);
        return Ok(eventDetails);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateEventAsync(Guid id, [FromBody] EventDetailsDto eventDetails, CancellationToken cancellationToken)
    {
        EventEntity? eventEntity = await _eventRepository.RetrieveAsync(id, cancellationToken);
        if (eventEntity is null)
        {
            _logger.LogInformation($"Event doesn't exist for this id: {id}");
            return NotFound("Event doesn't exist");
        }

        _mapper.Map(eventDetails, eventEntity);
        await _eventRepository.UpdateAsync(eventEntity, cancellationToken);
        await _eventRepository.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}   