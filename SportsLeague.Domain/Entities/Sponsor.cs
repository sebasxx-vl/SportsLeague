using SportsLeague.Domain.Enums;

namespace SportsLeague.Domain.Entities;

public class Sponsor : AuditBase
{
    public string? Name { get; set; }

    public string? ContactEmail { get; set; }

    public string? Phone { get; set; }

    public string? WebsiteUrl { get; set; }

    public SponsorCategory Category { get; set; }

    // Navigation Property (Relación N:M)
    public ICollection<TournamentSponsor> TournamentSponsors { get; set; }
        = new List<TournamentSponsor>();
}
