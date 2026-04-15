using Microsoft.EntityFrameworkCore;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;

namespace SportsLeague.DataAccess.Repositories;

public class TournamentSponsorRepository : GenericRepository<TournamentSponsor>, ITournamentSponsorRepository
{
    private readonly LeagueDbContext _context;
    public TournamentSponsorRepository(LeagueDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<TournamentSponsor?> GetByTournamentAndSponsorAsync(int tournamentId, int sponsorId)
    {
        return await _dbSet
            .Where(ts => ts.TournamentId == tournamentId && ts.SponsorId == sponsorId)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Tournament>> GetTournamentsBySponsorAsync(int sponsorId)
    {
        return await _dbSet
            .Where(ts => ts.SponsorId == sponsorId)
            .Include(ts => ts.Tournament)
            .Select(ts => ts.Tournament)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sponsor>> GetSponsorsByTournamentAsync(int tournamentId)
    {
        return await _dbSet
            .Where(ts => ts.TournamentId == tournamentId)
            .Include(ts => ts.Sponsor)
            .Select(ts => ts.Sponsor)
            .ToListAsync();
    }
}
