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
public class ThemeController : ControllerBase
{
    private readonly IThemeRepository _themeRepository;
    private readonly IPlannerRepository _plannerRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ThemeController> _logger;

    public ThemeController(
        IThemeRepository themeRepository,
        IMapper mapper,
        ILogger<ThemeController> logger,
        IPlannerRepository plannerRepository
    )
    {
        _themeRepository = themeRepository;
        _plannerRepository = plannerRepository;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateThemeAsync([FromBody] ThemeCreateDto theme, CancellationToken cancellationToken)
    {
        PlannerEntity? plannerEntity = await _plannerRepository.RetrieveAsync(theme.plannerId, cancellationToken);
        if (plannerEntity is null)
        {
            _logger.LogInformation($"Planner doesn't not exist with this id: {theme.plannerId}");
            return NotFound($"Planner doesn't not exist with this id: {theme.plannerId}");
        }

        ThemeEntity themeEntity = _mapper.Map<ThemeEntity>(theme);
        await _themeRepository.CreateAsync(themeEntity, cancellationToken);
        await _themeRepository.SaveChangesAsync(cancellationToken);

        ThemeDetailsDto themeDetails = _mapper.Map<ThemeDetailsDto>(themeEntity);
        return StatusCode(StatusCodes.Status201Created, themeDetails);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> RemoveThemeAsync(Guid id, CancellationToken cancellationToken)
    {
        ThemeEntity? themeEntity = await _themeRepository.RetrieveAsync(id, cancellationToken);
        if (themeEntity is null)
        {
            _logger.LogInformation($"Unable to retieve theme by id: {id}");
            return NotFound();
        }

        await _themeRepository.DeleteAsync(themeEntity, cancellationToken);
        await _themeRepository.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ThemeDetailsDto>>> ListThemesAsync(CancellationToken cancellationToken)
    {
        IEnumerable<ThemeEntity> themeEntities = await _themeRepository.ListAsync(cancellationToken);
        IEnumerable<ThemeDetailsDto> themes = _mapper.Map<IEnumerable<ThemeDetailsDto>>(themeEntities);
        return Ok(themes);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ThemeDetailsDto>> RetrieveThemeAsync(Guid id, CancellationToken cancellationToken)
    {
        ThemeEntity? themeEntity = await _themeRepository.RetrieveAsync(id, cancellationToken);
        if (themeEntity is null)
        {
            _logger.LogInformation($"Unable to retieve theme by id: {id}");
            return NotFound();
        }

        ThemeDetailsDto theme = _mapper.Map<ThemeDetailsDto>(themeEntity);
        return Ok(theme);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateThemeAsync(Guid id, [FromBody] ThemeDetailsDto theme, CancellationToken cancellationToken)
    {
        ThemeEntity? themeEntity = await _themeRepository.RetrieveAsync(id, cancellationToken);
        if (themeEntity is null)
        {
            _logger.LogInformation($"Unable to retieve theme by id: {id}");
            return NotFound();
        }

        _mapper.Map(theme, themeEntity);
        await _themeRepository.UpdateAsync(themeEntity, cancellationToken);
        await _themeRepository.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}   