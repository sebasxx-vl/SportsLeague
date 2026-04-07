namespace SportsLeague.Domain.Entities;
// Esta es una clase intemedia, un torneo puede tener muchos equipos y un equipo puede participar en muchos torneos.
public class TournamentTeam : AuditBase
{
    public int TournamentId { get; set; }  // Llave foránea: "¿a qué torneo pertenece?"
    public int TeamId { get; set; }        // Llave foránea: "¿qué equipo es?"
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;  // Fecha de inscripción

    // Navigation Properties
    public Tournament Tournament { get; set; } = null!;  // "A qué torneo pertenezco"
    public Team Team { get; set; } = null!;              // "Qué equipo soy"
}
