using AutoMapper;
using Joyful.API.Abstractions.Repositories;
using Joyful.API.Entities;
using Joyful.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Joyful.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class LocationController : ControllerBase
{
    private readonly ILocationRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<LocationController> _logger;

    public LocationController(ILocationRepository repository, IMapper mapper, ILogger<LocationController> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateLocationAsync([FromBody] LocationDto location, CancellationToken cancellationToken)
    {
        LocationEntity locationEntity = _mapper.Map<LocationEntity>(location);
        await _repository.CreateAsync(locationEntity, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        LocationDto returnedLocation = _mapper.Map<LocationDto>(locationEntity);

        return StatusCode(StatusCodes.Status201Created, returnedLocation);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteLocationAsync(Guid id, CancellationToken cancellationToken)
    {
        LocationEntity? locationEntity = await _repository.RetrieveAsync(id, cancellationToken);
        if (locationEntity is null)
        {
            _logger.LogInformation($"Location with location id: {id} not found");
            return NotFound($"Location with location id: {id} not found");
        }

        await _repository.DeleteAsync(locationEntity, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LocationDto>>> ListLocationsAsync(CancellationToken cancellationToken)
    {
        IEnumerable<LocationEntity> locationEntities = await _repository.ListAsync(cancellationToken);
        IEnumerable<LocationDto> locations = _mapper.Map<IEnumerable<LocationDto>>(locationEntities);
        return Ok(locations);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LocationDto>> RetrieveLocationAsync(Guid id, CancellationToken cancellationToken)
    {
        LocationEntity? locationEntity = await _repository.RetrieveAsync(id, cancellationToken);
        if (locationEntity is null)
        {
            _logger.LogInformation($"Location with location id: {id} not found");
            return NotFound($"Location with location id: {id} not found");
        }

        LocationDto location = _mapper.Map<LocationDto>(locationEntity);
        return Ok(location);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateLocationAsync(Guid id, LocationDto location, CancellationToken cancellationToken)
    {
        if (id != location.id)
        {
            _logger.LogError($"Id: {id} doesn't match location id: {location.id}");
            return BadRequest("Ids don't match");
        }

        LocationEntity? locationEntity = await _repository.RetrieveAsync(id, cancellationToken);
        if (locationEntity is null)
        {
            _logger.LogInformation($"Location with location id: {id} not found");
            return NotFound($"Location with location id: {id} not found");
        }

        _mapper.Map(location, locationEntity);
        await _repository.UpdateAsync(locationEntity, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}