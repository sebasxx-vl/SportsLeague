using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SponsorController : ControllerBase
{
    private readonly ISponsorService _sponsorService;
    private readonly IMapper _mapper;

    public SponsorController(ISponsorService sponsorService, IMapper mapper)
    {
        _sponsorService = sponsorService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var sponsors = await _sponsorService.GetAllAsync();
        var dto = _mapper.Map<IEnumerable<SponsorResponseDTO>>(sponsors);
        return Ok(dto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var sponsor = await _sponsorService.GetByIdAsync(id);
        if (sponsor == null) return NotFound();
        return Ok(_mapper.Map<SponsorResponseDTO>(sponsor));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SponsorRequestDTO request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var sponsor = _mapper.Map<Sponsor>(request);
            var created = await _sponsorService.CreateAsync(sponsor);
            var dto = _mapper.Map<SponsorResponseDTO>(created);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] SponsorRequestDTO request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var sponsor = _mapper.Map<Sponsor>(request);
            await _sponsorService.UpdateAsync(id, sponsor);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _sponsorService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // --- Tournament links ---

    [HttpGet("{id}/tournaments")]
    public async Task<IActionResult> GetTournaments(int id)
    {
        try
        {
            var tournaments = await _sponsorService.GetTournamentsBySponsorAsync(id);
            var dto = _mapper.Map<IEnumerable<TournamentResponseDTO>>(tournaments);
            return Ok(dto);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/tournaments")]
    public async Task<IActionResult> LinkToTournament(int id, [FromBody] TournamentSponsorRequestDTO request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var ts = new TournamentSponsor
            {
                TournamentId = request.TournamentId,
                ContractAmount = request.ContractAmount
            };

            var created = await _sponsorService.LinkToTournamentAsync(id, ts);

            // get tournament name from sponsor's tournaments list
            var tournaments = await _sponsorService.GetTournamentsBySponsorAsync(id);
            var tournament = tournaments.FirstOrDefault(t => t.Id == request.TournamentId);

            // get sponsor name
            var sponsor = await _sponsorService.GetByIdAsync(id);

            var response = new TournamentSponsorResponseDTO
            {
                Id = created.Id,
                TournamentId = created.TournamentId,
                TournamentName = tournament?.Name ?? string.Empty,
                SponsorId = created.SponsorId,
                SponsorName = sponsor?.Name ?? string.Empty,
                ContractAmount = created.ContractAmount,
                JoinedAt = created.JoinedAt
            };

            return CreatedAtAction(nameof(GetTournaments), new { id = id }, response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}/tournaments/{tid}")]
    public async Task<IActionResult> UnlinkFromTournament(int id, int tid)
    {
        try
        {
            await _sponsorService.UnlinkFromTournamentAsync(id, tid);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
