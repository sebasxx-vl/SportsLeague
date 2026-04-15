namespace SportsLeague.Domain.Entities;

public class TournamentSponsor : AuditBase
{
    public int TournamentId { get; set; }
    public Tournament Tournament { get; set; } = null!;

    public int SponsorId { get; set; }
    public Sponsor Sponsor { get; set; } = null!;
}
