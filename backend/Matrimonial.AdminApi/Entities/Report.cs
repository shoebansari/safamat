namespace Matrimonial.AdminApi.Entities;

public class Report
{
    public Guid ReportId { get; set; }
    public Guid ReporterUserId { get; set; }
    public Guid ReportedUserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public User Reporter { get; set; } = null!;
    public User Reported { get; set; } = null!;
}
