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
public class PollController : ControllerBase
{
    private readonly IPollRepository _pollRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PollController> _logger;

    public PollController(
        IPollRepository pollRepository,
        IEventRepository eventRepository,
        IMapper mapper,
        ILogger<PollController> logger
    )
    {
        _pollRepository = pollRepository;
        _eventRepository = eventRepository;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult> CreatePollAsync([FromBody] PollCreateDto poll, CancellationToken cancellationToken)
    {
        EventEntity? eventEntity = await _eventRepository.RetrieveAsync(poll.EventId, cancellationToken);
        if (eventEntity is null)
        {
            _logger.LogInformation($"Unable to find event with this event id: {poll.EventId}");
            return NotFound();
        }

        PollEntity pollEntity = _mapper.Map<PollEntity>(poll);
        await _pollRepository.CreateAsync(pollEntity, cancellationToken);
        await _pollRepository.SaveChangesAsync(cancellationToken);

        PollDetailsDto pollDetails = _mapper.Map<PollDetailsDto>(pollEntity);
        return Ok(pollDetails);
    }

    [HttpGet("{id:guid}/list-events")]
    public async Task<ActionResult<IEnumerable<PollDetailsDto>>> ListPollsForEventAsync(Guid id, CancellationToken cancellationToken)
    {
        IEnumerable<PollEntity> pollEntities = await _pollRepository.ListPollsForEventAsync(id, cancellationToken);
        IEnumerable<PollDetailsDto> polls = _mapper.Map<IEnumerable<PollDetailsDto>>(pollEntities);
        return Ok(polls);
    }

    [HttpGet("{eventId:guid}/{pollId:guid}")]
    public async Task<ActionResult<PollDetailsDto>> RetrievePollForEventAsync(Guid eventId, Guid pollId, CancellationToken cancellationToken)
    {
        EventEntity? eventEntity = await _eventRepository.RetrieveAsync(eventId, cancellationToken);
        if (eventEntity is null)
        {
            _logger.LogInformation($"Unable to find event with this event id: {eventId}");
            return NotFound();
        }

        PollEntity? pollEntity = await _pollRepository.RetrievePollForEventAsync(eventId, pollId, cancellationToken);

        if (pollEntity is null)
        {
            _logger.LogInformation($"Unable to find poll with this poll id: {pollId}");
            return NotFound();
        }

        PollDetailsDto pollDetails = _mapper.Map<PollDetailsDto>(pollEntity);
        return Ok(pollDetails);
    }

    [HttpPut("{eventId:guid}/{pollId:guid}")]
    public async Task<IActionResult> UpdatePollForEventAsync(Guid eventId, Guid pollId, [FromBody] PollCreateDto pollDto, CancellationToken cancellationToken)
    {
        EventEntity? eventEntity = await _eventRepository.RetrieveAsync(eventId, cancellationToken);
        if (eventEntity is null)
        {
            _logger.LogInformation($"Unable to find event with this event id: {eventId}");
            return NotFound();
        }

        PollEntity? pollEntity = await _pollRepository.RetrievePollForEventAsync(eventId, pollId, cancellationToken);
        if (pollEntity is null)
        {
            _logger.LogInformation($"Unable to find poll with this poll id: {pollId}");
            return NotFound();
        }

        _mapper.Map(pollDto, pollEntity);
        await _pollRepository.UpdateAsync(pollEntity, cancellationToken);
        await _pollRepository.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpDelete("{eventId:guid}/{pollId:guid}")]
    public async Task<IActionResult> DeletePollForEventAsync(Guid eventId, Guid pollId, CancellationToken cancellationToken)
    {
        EventEntity? eventEntity = await _eventRepository.RetrieveAsync(eventId, cancellationToken);
        if (eventEntity is null)
        {
            _logger.LogInformation($"Unable to find event with this event id: {eventId}");
            return NotFound();
        }

        PollEntity? pollEntity = await _pollRepository.RetrievePollForEventAsync(eventId, pollId, cancellationToken);
        if (pollEntity is null)
        {
            _logger.LogInformation($"Unable to find poll with this poll id: {pollId}");
            return NotFound();
        }

        await _pollRepository.DeleteAsync(pollEntity, cancellationToken);
        await _pollRepository.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

}