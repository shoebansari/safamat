namespace Matrimonial.AdminApi.Entities;

public class Match
{
    public Guid MatchId { get; set; }
    public Guid UserId1 { get; set; }
    public Guid UserId2 { get; set; }
    public decimal MatchPercentage { get; set; }
    public DateTime MatchedOn { get; set; } = DateTime.UtcNow;

    public User User1 { get; set; } = null!;
    public User User2 { get; set; } = null!;
}
