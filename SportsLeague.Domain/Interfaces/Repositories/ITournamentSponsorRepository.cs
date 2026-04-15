using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Repositories;

public interface ITournamentSponsorRepository : IGenericRepository<TournamentSponsor>
{
    Task<TournamentSponsor?> GetByTournamentAndSponsorAsync(int tournamentId, int sponsorId);
    Task<IEnumerable<Tournament>> GetTournamentsBySponsorAsync(int sponsorId);
    Task<IEnumerable<Sponsor>> GetSponsorsByTournamentAsync(int tournamentId);
}
