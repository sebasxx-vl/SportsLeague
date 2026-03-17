using SportsLeague.Domain.Enums;

namespace SportsLeague.Domain.Entities
{
    public class Player : AuditBase
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public int Number { get; set; }
        public PlayerPosition Position { get; set; }

        // Foreign Key - Relación entre Player y Team
        public int TeamId { get; set; }

        // Navigation Property - Permite obtener y recuperar datos del jugador junto con su equipo
        public Team Team { get; set; } = null!;
    }
}
