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
        var sponsor = _mapper.Map<Sponsor>(request);
        var created = await _sponsorService.CreateAsync(sponsor);
        var dto = _mapper.Map<SponsorResponseDTO>(created);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] SponsorRequestDTO request)
    {
        var sponsor = _mapper.Map<Sponsor>(request);
        await _sponsorService.UpdateAsync(id, sponsor);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _sponsorService.DeleteAsync(id);
        return NoContent();
    }

    // --- Tournament links ---
    [HttpGet("{id}/tournaments")]
    public async Task<IActionResult> GetTournaments(int id)
    {
        var tournaments = await _sponsorService.GetTournamentsBySponsorAsync(id);
        var dto = _mapper.Map<IEnumerable<TournamentResponseDTO>>(tournaments);
        return Ok(dto);
    }

    [HttpPost("{id}/tournaments")]
    public async Task<IActionResult> LinkToTournament(int id, [FromBody] TournamentSponsorRequestDTO request)
    {
        var ts = new TournamentSponsor
        {
            TournamentId = request.TournamentId,
            ContractAmount = request.ContractAmount
        };
        var created = await _sponsorService.LinkToTournamentAsync(id, ts);
        var dto = _mapper.Map<TournamentSponsorResponseDTO>(created);
        return CreatedAtAction(nameof(GetTournaments), new { id = id }, dto);
    }

    [HttpDelete("{id}/tournaments/{tid}")]
    public async Task<IActionResult> UnlinkFromTournament(int id, int tid)
    {
        await _sponsorService.UnlinkFromTournamentAsync(id, tid);
        return NoContent();
    }
}
