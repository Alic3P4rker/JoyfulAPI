using Joyful.API.Abstractions.Repositories;
using Joyful.API.Enums;
using Joyful.API.Entities;
using Joyful.API.Models;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Text.Json;

namespace Joyful.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VoteController : ControllerBase
{
    private readonly IPollRepository _pollRepository;
    private readonly IVoteRepository _voteRepository;
    private readonly IPlannerRepository _plannerRepository;
    private readonly ILogger<VoteController> _logger;
    private readonly IMapper _mapper;

    public VoteController(
        IPollRepository pollRepository,
        IVoteRepository voteRepository,
        IPlannerRepository plannerRepository,
        ILogger<VoteController> logger,
        IMapper mapper
    )
    {
        _plannerRepository = plannerRepository;
        _pollRepository = pollRepository;
        _voteRepository = voteRepository;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost("{pollId:guid}/{voterId:guid}/register-vote")]
    public async Task<ActionResult> RegisterVoteAsync(Guid pollId, Guid voterId, [FromBody] VoteDto vote, CancellationToken cancellationToken)
    {
        //Step 1: Retrieve poll
        PollEntity? pollEntity = await _pollRepository.RetrievePollAsync(pollId, cancellationToken);
        if (pollEntity is null)
        {
            _logger.LogInformation($"Can't find poll with poll id: {pollId}");
            return NotFound("Unable to retrieve poll");
        }

        //Step 1.5: Verify planner
        PlannerEntity? plannerEntity = await _plannerRepository.RetrieveAsync(voterId, cancellationToken);
        if (plannerEntity is null)
        {
            _logger.LogInformation($"Unable to find voter with this voter id: {voterId}");
            return NotFound("Unable to retrive voter");
        }

        //Step 2: Verify poll status
            if (pollEntity.PollStatus != PollStatus.Open)
            {
                _logger.LogInformation("Voting is not open");
                return Unauthorized("Voting closed");
            }

        //Step 3: Getting pollOptions
        List<string>? pollOptions = JsonSerializer.Deserialize<List<string>>(pollEntity.OptionsJson);
        if (pollOptions is null)
        {
            _logger.LogInformation("Not options are available for this poll");
            return NotFound("Not options are available for this poll");
        }
#warning implement multiple voting restrictions

        //Step 4: Validate 
        foreach (string chosenSuggestion in vote.ChosenSuggestions)
        {
            if (!pollOptions.Contains(chosenSuggestion))
            {
                _logger.LogInformation($"Submitted suggestion '{chosenSuggestion}' is not a valid option for Poll {pollId}.", chosenSuggestion, pollId);
                return BadRequest($"Submitted suggestion '{chosenSuggestion}' is not a valid option for Poll {pollId}.");
            }
        }

        if (pollEntity.PollType != PollType.MultiSelect && vote.ChosenSuggestions.Count > 1)
        {
            _logger.LogInformation($"Poll with poll id {pollId} does not allow multiple chooses");
            return BadRequest("Poll doesn't allow multiple chose");
        }

        //Step 4: Register Vote 
        string chosenSuggestionJson = JsonSerializer.Serialize(vote.ChosenSuggestions);
        VoteEntity voteEntity = _mapper.Map<VoteEntity>(vote);
        voteEntity.VoterId = voterId;
        voteEntity.PollId = pollId;
        voteEntity.ChosenSuggestionsJson = chosenSuggestionJson;
        voteEntity.VotedAt = DateTimeOffset.Now;

        await _voteRepository.CreateAsync(voteEntity, cancellationToken);
        await _voteRepository.SaveChangesAsync(cancellationToken);

        return Ok("Vote Submitted");
    }

    [HttpGet("{pollId:guid}/get-poll-results")]
    public async Task<ActionResult> GetPollResultsAsync(Guid pollId, CancellationToken cancellationToken)
    {
        PollEntity? pollEntity = await _pollRepository.RetrievePollAsync(pollId, cancellationToken);
        if (pollEntity is null)
        {
            _logger.LogInformation($"Can't find poll with poll id: {pollId}");
            return NotFound("Unable to retrieve poll");
        }

        List<string>? pollOptions = JsonSerializer.Deserialize<List<string>>(pollEntity.OptionsJson);
        if (pollOptions is null)
        {
            _logger.LogInformation("No options are available for this poll");
            return NotFound("No options are available for this poll");
        }

        IEnumerable<VoteEntity> votes = await _voteRepository.ListVotesForPollAsync(pollId, cancellationToken);

        Dictionary<string, int> optionVotes = new Dictionary<string, int>();
        int totalSubmissions = 0;

        foreach (VoteEntity vote in votes)
        {
            List<string>? chosenSuggestions = JsonSerializer.Deserialize<List<string>>(vote.ChosenSuggestionsJson);

            if (chosenSuggestions is not null)
            {
                foreach (string suggestionText in chosenSuggestions)
                {
                    if (optionVotes.ContainsKey(suggestionText))
                    {
                        optionVotes[suggestionText]++;
                    }
                    else
                    {
                        optionVotes[suggestionText] = 1;
                    }
                }

                totalSubmissions++;
            }
        }

        List<OptionResultDto> optionResults = new List<OptionResultDto>();
        foreach (string optionText in pollOptions)
        {
            OptionResultDto optionResult = new OptionResultDto();
            optionResult.OptionText = optionText;
            optionResult.VoteCount = optionVotes.ContainsKey(optionText) ? optionVotes[optionText] : 0;

            optionResults.Add(optionResult);
        }

        optionResults = optionResults.OrderByDescending(o => o.VoteCount).ToList();

        PollResultsDto pollResultsDto = new PollResultsDto();
        pollResultsDto.PollId = pollId;
        pollResultsDto.Question = pollEntity.Question;
        pollResultsDto.TotalVotes = totalSubmissions;
        pollResultsDto.OptionResults = optionResults;


        return Ok(pollResultsDto);
    }
}