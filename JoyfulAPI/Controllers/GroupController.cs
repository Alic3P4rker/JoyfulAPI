using AutoMapper;
using Joyful.API.Abstractions.Repositories;
using Joyful.API.Entities;
using Joyful.API.Models;
using Joyful.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Joyful.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GroupController : ControllerBase
{
    private readonly IGroupRepository _groupRepository;
    private readonly IPlannerRepository _plannerRepository;
    private readonly IPlannerGroupRepository _plannerGroupRepository;
    private readonly ILogger<GroupController> _logger;
    private readonly IMapper _mapper;

    public GroupController(
        IPlannerRepository plannerRepository,
        IPlannerGroupRepository plannerGroupRepository,
        IGroupRepository repository,
        ILogger<GroupController> logger,
        IMapper mapper)
    {
        _plannerRepository = plannerRepository;
        _plannerGroupRepository = plannerGroupRepository;
        _groupRepository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> CreateGroupAsync([FromBody] GroupDto group, CancellationToken cancellationToken)
    {
        //Step 1: Verify Group doesn't exist
        GroupEntity? tempGroup = await _groupRepository.RetrieveAsync(group.id, cancellationToken);
        if (tempGroup is not null)
        {
            _logger.LogError($"Group with Group id: {group.id} already exists");
            return Conflict($"Group with Group id: {group.id} already exists");
        }

        //Step 2: Verify Planner already exists
        PlannerEntity? plannerEntity = await _plannerRepository.RetrieveAsync(group.groupLeaderId, cancellationToken);
        if (plannerEntity is null)
        {
            _logger.LogError($"Attempted to create a group with non-existent planner id: {group.groupLeaderId}");
            return BadRequest($"Invalid GroupLeaderId: Planner with ID {group.groupLeaderId} does not exist.");
        }

        //Step 3: Generate access code
        string accessCode = RandomCodeGeneratorService.GenerateRandomAlphanumericCode(6);
        GroupEntity groupEntity = _mapper.Map<GroupEntity>(group);

        //Step 3.5: Create group
        groupEntity.AccessCode = accessCode;
        await _groupRepository.CreateAsync(groupEntity, cancellationToken);

        PlannerGroupEntity plannerGroupEntity = new PlannerGroupEntity();
        plannerGroupEntity.GroupId = groupEntity.Id;
        plannerGroupEntity.PlannerId = groupEntity.GroupLeaderId;

        //Step 4: Save
        await _plannerGroupRepository.CreateAsync(plannerGroupEntity, cancellationToken);
        await _groupRepository.SaveChangesAsync(cancellationToken);

        GroupDto returnedGroupDto = _mapper.Map<GroupDto>(groupEntity);

        _logger.LogInformation($"Group successfully created, group id: {groupEntity.Id}");
        return StatusCode(StatusCodes.Status201Created, returnedGroupDto);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteGroupAsync(Guid id, CancellationToken cancellationToken)
    {
        GroupEntity? groupEntity = await _groupRepository.RetrieveAsync(id, cancellationToken);
        if (groupEntity is null)
        {
            _logger.LogError($"Group with group id: {id} not found");
            return NotFound("Group not Found");
        }

        await _groupRepository.DeleteAsync(groupEntity, cancellationToken);
        await _groupRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Group successfully removed");
        return NoContent();
    }

    [HttpGet("list-groups")]
    public async Task<ActionResult<IEnumerable<GroupDto>>> ListGroupsAsync(CancellationToken cancellationToken)
    {
        IEnumerable<GroupEntity> groupEntities = await _groupRepository.ListAsync(cancellationToken);
        IEnumerable<GroupDto> groups = _mapper.Map<IEnumerable<GroupDto>>(groupEntities);
        _logger.LogInformation("Groups successfully returned");
        return Ok(groups);
    }

    [HttpDelete]
    public async Task<ActionResult<GroupDto>> RetrieveGroupAsync(Guid id, CancellationToken cancellationToken)
    {
        GroupEntity? groupEntity = await _groupRepository.RetrieveAsync(id, cancellationToken);
        if (groupEntity is null)
        {
            _logger.LogError($"Group with group id: {id} not found");
            return NotFound("Group not Found");
        }

        GroupDto group = _mapper.Map<GroupDto>(groupEntity);
        _logger.LogInformation($"Group successfully returned");
        return Ok(group);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateGroupAsync(Guid id, [FromBody] GroupDto group, CancellationToken cancellationToken)
    {
        if (id != group.id)
        {
            _logger.LogError($"Id: {id} doesn't match group id: {group.id}");
            return BadRequest("Ids don't match");
        }

        GroupEntity? groupEntity = await _groupRepository.RetrieveAsync(id, cancellationToken);
        if (groupEntity is null)
        {
            _logger.LogError($"Group with group id: {id} not found");
            return NotFound("Group not Found");
        }

        _mapper.Map(group, groupEntity);
        await _groupRepository.UpdateAsync(groupEntity, cancellationToken);
        await _groupRepository.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Group successfully updated");

        return NoContent();
    }

    [HttpGet("list-groups-for-planners/{id:guid}")]
    public async Task<ActionResult<IEnumerable<GroupDto>>> ListGroupsForPlanner(Guid id, CancellationToken cancellationToken)
    {
        IEnumerable<GroupEntity> groupEntities = await _plannerGroupRepository.ListGroupsByPlannerId(id, cancellationToken);
        IEnumerable<GroupDto> groups = _mapper.Map<IEnumerable<GroupDto>>(groupEntities);
        _logger.LogInformation("Groups successfully created");
        return Ok(groups);
    }

    [HttpGet("retrieve-group-for-planner/{plannerId:guid}/{groupId:guid}")]
    public async Task<ActionResult<GroupDto>> RetrieveGroupForPlanner(Guid plannerId, Guid groupId, CancellationToken cancellationToken)
    {
        GroupEntity? groupEntity = await _plannerGroupRepository.RetrieveGroup(plannerId, groupId, cancellationToken);
        if (groupEntity is null)
        {
            _logger.LogError($"Unable to find group with planner id: {plannerId} and group id: {groupId}");
            return NotFound("Unable to find group");
        }

        GroupDto group = _mapper.Map<GroupDto>(groupEntity);
        return Ok(group);
    }

    [HttpPost("/join-group/{plannerId:guid}/{groupId:guid}")]
    public async Task<IActionResult> JoinGroupAsync(Guid plannerId, Guid groupId, CancellationToken cancellationToken)
    {
        GroupEntity? groupEntity = await _groupRepository.RetrieveAsync(groupId, cancellationToken);
        if (groupEntity is null)
        {
            _logger.LogError($"Group with group id: {groupId} not found");
            return NotFound("Group not Found");
        }

        PlannerEntity? plannerEntity = await _plannerRepository.RetrieveAsync(plannerId, cancellationToken);
        if (plannerEntity is null)
        {
            _logger.LogError($"Planner with planner id: {plannerId} not found");
            return NotFound("Planner not Found");
        }

        PlannerGroupEntity plannerGroupEntity = new PlannerGroupEntity();
        plannerGroupEntity.PlannerId = plannerId;
        plannerGroupEntity.GroupId = groupId;

        await _plannerGroupRepository.CreateAsync(plannerGroupEntity, cancellationToken);

        return Ok("Successfully joined group");
    }
}