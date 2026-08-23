namespace Matrimonial.AdminApi.Entities;

public class UserOccupation
{
    public Guid OccupationId { get; set; }
    public Guid UserId { get; set; }
    public string? Occupation { get; set; }
    public string? CompanyName { get; set; }
    public string? Designation { get; set; }
    public decimal? AnnualIncome { get; set; }
    public string? WorkLocation { get; set; }

    public User User { get; set; } = null!;
}
