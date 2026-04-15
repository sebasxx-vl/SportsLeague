namespace SportsLeague.API.DTOs.Request;

public class TournamentSponsorRequestDTO
{
    public int TournamentId { get; set; }
    [System.ComponentModel.DataAnnotations.Range(0.01, double.MaxValue, ErrorMessage = "ContractAmount must be greater than 0")]
    public decimal ContractAmount { get; set; }
}
