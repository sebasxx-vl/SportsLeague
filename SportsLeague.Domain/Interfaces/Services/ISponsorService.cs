using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Services;

public interface ISponsorService
{
    Task<IEnumerable<Sponsor>> GetAllAsync();
    Task<Sponsor?> GetByIdAsync(int id);
    Task<Sponsor> CreateAsync(Sponsor sponsor);
    Task UpdateAsync(int id, Sponsor sponsor);
    Task DeleteAsync(int id);

    // Tournament-sponsor related
    Task<TournamentSponsor> LinkToTournamentAsync(int sponsorId, TournamentSponsor tournamentSponsor);
    Task<IEnumerable<Tournament>> GetTournamentsBySponsorAsync(int sponsorId);
    Task UnlinkFromTournamentAsync(int sponsorId, int tournamentId);
}
