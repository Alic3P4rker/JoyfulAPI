using AutoMapper;
using Joyful.API.Abstractions.Repositories;
using Joyful.API.Entities;
using Joyful.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Joyful.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlannerController : ControllerBase
{
    private readonly IPlannerRepository _plannerRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PlannerController> _logger;

    public PlannerController(
        IPlannerRepository plannerRepository,
        IMapper mapper,
        ILogger<PlannerController> logger,
        IUserRepository userRepository
    )
    {
        _plannerRepository = plannerRepository;
        _mapper = mapper;
        _logger = logger;
        _userRepository = userRepository;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePlannerAsync([FromBody] PlannerDto planner, CancellationToken cancellationToken)
    {
        UserEntity? userEntity = await _userRepository.RetrieveByEmailAsync(planner.emailAddress, cancellationToken);
#warning might need to inspect this later on
        if (userEntity is null)
        {
            _logger.LogError($"Unable to find user with email address: {planner.emailAddress}");
            return NotFound($"Unable to find user with email address: {planner.emailAddress}");
        }

        PlannerEntity? tempPlannerEntity = await _plannerRepository.RetrieveAsync(planner.id, cancellationToken);
        if (tempPlannerEntity is not null)
        {
            _logger.LogError($"Planner with Planner Id: {planner.id} already exists");
            return Conflict($"Planner with Planner Id: {planner.id} already exists");
        }

        PlannerEntity plannerEntity = _mapper.Map<PlannerEntity>(planner);
        await _plannerRepository.CreateAsync(plannerEntity, cancellationToken);
        await _plannerRepository.SaveChangesAsync(cancellationToken);

        PlannerDto returnedPlanner = _mapper.Map<PlannerDto>(plannerEntity);

        return StatusCode(StatusCodes.Status201Created, returnedPlanner);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeletePlannerAsync(Guid id, CancellationToken cancellationToken)
    {
        PlannerEntity? plannerEntity = await _plannerRepository.RetrieveAsync(id, cancellationToken);
        if (plannerEntity is null)
        {
            _logger.LogError($"Planner with id: {id} not found");
            return NotFound($"Planner with id: {id} not found");
        }

        await _plannerRepository.DeleteAsync(plannerEntity, cancellationToken);
        await _plannerRepository.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlannerDto>>> ListPlannersAsync(CancellationToken cancellationToken)
    {
        IEnumerable<PlannerEntity> plannerEntities = await _plannerRepository.ListAsync(cancellationToken);
        IEnumerable<PlannerDto> planners = _mapper.Map<IEnumerable<PlannerDto>>(plannerEntities);
        _logger.LogInformation("Planners successfully retrieved");
        return Ok(planners);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PlannerDto>> RetrievePlannerAsync(Guid id, CancellationToken cancellationToken)
    {
        PlannerEntity? plannerEntity = await _plannerRepository.RetrieveAsync(id, cancellationToken);
        if (plannerEntity is null)
        {
            _logger.LogError($"Planner with id: {id} not found");
            return NotFound($"Planner with id: {id} not found");
        }

        PlannerDto planner = _mapper.Map<PlannerDto>(plannerEntity);
        _logger.LogInformation($"Planner with id: {id} successfully found");
        return Ok(planner);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdatePlannerAsync(Guid id, [FromBody] PlannerDto planner, CancellationToken cancellationToken)
    {
        if (id != planner.id)
        {
            _logger.LogError($"Id: {id} doesn't match planner id: {planner.id}");
            return BadRequest("Ids don't match");
        }

        PlannerEntity? plannerEntity = await _plannerRepository.RetrieveAsync(id, cancellationToken);
        if (plannerEntity is null)
        {
            _logger.LogError($"Planner with id: {id} not found");
            return NotFound($"Planner with id: {id} not found");
        }

        _mapper.Map(planner, plannerEntity);
        await _plannerRepository.UpdateAsync(plannerEntity, cancellationToken);
        await _plannerRepository.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}