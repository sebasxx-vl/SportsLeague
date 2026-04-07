using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Services;

public interface IRefereeService
{
    Task<IEnumerable<Referee>> GetAllAsync(); // Dame TODOS los árbitros
    Task<Referee?> GetByIdAsync(int id); // Dame UNO por id  (el ? significa que puede no existir)
    Task<Referee> CreateAsync(Referee referee); // Crea un árbitro nuevo
    Task UpdateAsync(int id, Referee referee); // Actualiza un árbitro
    Task DeleteAsync(int id); // Elimina un árbitro

}
