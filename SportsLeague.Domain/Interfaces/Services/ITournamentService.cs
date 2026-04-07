using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;

//ITournamentService tiene métodos que van más allá del CRUD: UpdateStatusAsync para cambiar el estado con validaciones de 
//transición, RegisterTeamAsync para inscribir equipos con validaciones, y GetTeamsByTournamentAsync 
//para consultar los equipos inscritos.
namespace SportsLeague.Domain.Interfaces.Services;
public interface ITournamentService
{
    Task<IEnumerable<Tournament>> GetAllAsync(); // Dame TODOS los torneos
    Task<Tournament?> GetByIdAsync(int id); // Dame UNO por id (el ? significa que puede no existir)
    Task<Tournament> CreateAsync(Tournament tournament); // Crea un torneo nuevo
    Task UpdateAsync(int id, Tournament tournament); // Actualiza un torneo
    Task DeleteAsync(int id); // Elimina un torneo
    Task UpdateStatusAsync(int id, TournamentStatus newStatus); // Actualiza el estado del torneo (Pending, Active, Finished, Cancelled)
    Task RegisterTeamAsync(int tournamentId, int teamId); // Registra un equipo en un torneo
    Task<IEnumerable<Team>> GetTeamsByTournamentAsync(int tournamentId); // Obtiene todos los equipos de un torneo
}
