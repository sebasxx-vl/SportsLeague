using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services;

public class SponsorService : ISponsorService
{
    private readonly ISponsorRepository _sponsorRepository;
    private readonly ITournamentRepository _tournamentRepository;
    private readonly ITournamentSponsorRepository _tournamentSponsorRepository;
    private readonly ILogger<SponsorService> _logger;

    public SponsorService(
        ISponsorRepository sponsorRepository,
        ITournamentRepository tournamentRepository,
        ITournamentSponsorRepository tournamentSponsorRepository,
        ILogger<SponsorService> logger)
    {
        _sponsorRepository = sponsorRepository;
        _tournamentRepository = tournamentRepository;
        _tournamentSponsorRepository = tournamentSponsorRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Sponsor>> GetAllAsync()
    {
        _logger.LogInformation("Retrieving all sponsors");
        return await _sponsorRepository.GetAllAsync();
    }

    public async Task<Sponsor?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Retrieving sponsor with ID: {SponsorId}", id);
        return await _sponsorRepository.GetByIdAsync(id);
    }

    public async Task<Sponsor> CreateAsync(Sponsor sponsor)
    {
        // Validar nombre único
        if (await _sponsorRepository.ExistsByNameAsync(sponsor.Name ?? string.Empty))
        {
            _logger.LogWarning("Sponsor with name '{SponsorName}' already exists", sponsor.Name);
            throw new InvalidOperationException($"Ya existe un sponsor con el nombre '{sponsor.Name}'");
        }

        // Validar email (básico)
        try
        {
            var addr = new System.Net.Mail.MailAddress(sponsor.ContactEmail ?? string.Empty);
            if (addr.Address != sponsor.ContactEmail)
                throw new InvalidOperationException("Invalid email format");
        }
        catch
        {
            throw new InvalidOperationException("ContactEmail must be a valid email address");
        }

        _logger.LogInformation("Creating sponsor: {SponsorName}", sponsor.Name);
        return await _sponsorRepository.CreateAsync(sponsor);
    }

    public async Task UpdateAsync(int id, Sponsor sponsor)
    {
        var existing = await _sponsorRepository.GetByIdAsync(id);
        if (existing == null)
        {
            _logger.LogWarning("Sponsor with ID {SponsorId} not found for update", id);
            throw new KeyNotFoundException($"Sponsor not found with ID {id}");
        }

        // Validar email
        try
        {
            var addr = new System.Net.Mail.MailAddress(sponsor.ContactEmail ?? string.Empty);
            if (addr.Address != sponsor.ContactEmail)
                throw new InvalidOperationException("Invalid email format");
        }
        catch
        {
            throw new InvalidOperationException("ContactEmail must be a valid email address");
        }

        // Validar nombre duplicado si cambió
        if (!string.Equals(existing.Name, sponsor.Name, StringComparison.OrdinalIgnoreCase))
        {
            if (await _sponsorRepository.ExistsByNameAsync(sponsor.Name ?? string.Empty))
            {
                throw new InvalidOperationException("A sponsor with the same name already exists");
            }
        }

        existing.Name = sponsor.Name;
        existing.ContactEmail = sponsor.ContactEmail;
        existing.Phone = sponsor.Phone;
        existing.WebsiteUrl = sponsor.WebsiteUrl;
        existing.Category = sponsor.Category;

        _logger.LogInformation("Updating sponsor with ID: {SponsorId}", id);
        await _sponsorRepository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        var exists = await _sponsorRepository.ExistsAsync(id);
        if (!exists)
        {
            _logger.LogWarning("Sponsor with ID {SponsorId} not found for deletion", id);
            throw new KeyNotFoundException($"Sponsor not found with ID {id}");
        }

        _logger.LogInformation("Deleting sponsor with ID: {SponsorId}", id);
        await _sponsorRepository.DeleteAsync(id);
    }

    public async Task<TournamentSponsor> LinkToTournamentAsync(int sponsorId, TournamentSponsor tournamentSponsor)
    {
        // Validar sponsor existe
        var sponsorExists = await _sponsorRepository.ExistsAsync(sponsorId);
        if (!sponsorExists)
            throw new KeyNotFoundException($"Sponsor not found with ID {sponsorId}");

        // Validar torneo existe
        var tournamentExists = await _tournamentRepository.ExistsAsync(tournamentSponsor.TournamentId);
        if (!tournamentExists)
            throw new KeyNotFoundException($"Tournament not found with ID {tournamentSponsor.TournamentId}");

        // Validar ContractAmount > 0
        if (tournamentSponsor.ContractAmount <= 0)
            throw new InvalidOperationException("ContractAmount must be greater than 0");

        // Validar duplicado
        var existing = await _tournamentSponsorRepository.GetByTournamentAndSponsorAsync(
            tournamentSponsor.TournamentId, sponsorId);
        if (existing != null)
            throw new InvalidOperationException("Sponsor already linked to tournament");

        // Crear
        tournamentSponsor.SponsorId = sponsorId;
        var created = await _tournamentSponsorRepository.CreateAsync(tournamentSponsor);

        // Recargar con navigation properties y devolver
        var reloaded = await _tournamentSponsorRepository.GetByTournamentAndSponsorAsync(
            tournamentSponsor.TournamentId, sponsorId);
        if (reloaded == null)
            return created; // fallback

        return reloaded;
    }

    public async Task<IEnumerable<Tournament>> GetTournamentsBySponsorAsync(int sponsorId)
    {
        var sponsorExists = await _sponsorRepository.ExistsAsync(sponsorId);
        if (!sponsorExists)
            throw new KeyNotFoundException($"Sponsor not found with ID {sponsorId}");

        return await _tournamentSponsorRepository.GetTournamentsBySponsorAsync(sponsorId);
    }

    public async Task UnlinkFromTournamentAsync(int sponsorId, int tournamentId)
    {
        var sponsorExists = await _sponsorRepository.ExistsAsync(sponsorId);
        if (!sponsorExists)
            throw new KeyNotFoundException($"Sponsor not found with ID {sponsorId}");

        var existing = await _tournamentSponsorRepository.GetByTournamentAndSponsorAsync(tournamentId, sponsorId);
        if (existing == null)
            throw new KeyNotFoundException("Link not found");

        await _tournamentSponsorRepository.DeleteAsync(existing.Id);
    }
}
