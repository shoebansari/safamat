namespace Matrimonial.AdminApi.Entities;

public class UserEducation
{
    public Guid EducationId { get; set; }
    public Guid UserId { get; set; }
    public string? Qualification { get; set; }
    public string? College { get; set; }
    public string? University { get; set; }
    public int? PassingYear { get; set; }
    public string? EducationType { get; set; }

    public User User { get; set; } = null!;
}
