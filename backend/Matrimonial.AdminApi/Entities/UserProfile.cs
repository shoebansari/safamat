namespace Matrimonial.AdminApi.Entities;

public class UserProfile
{
    public Guid ProfileId { get; set; }
    public Guid UserId { get; set; }
    public string? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public decimal? Height { get; set; }
    public decimal? Weight { get; set; }
    public string? MaritalStatus { get; set; }
    public string? Religion { get; set; }
    public string? Caste { get; set; }
    public string? SubCaste { get; set; }
    public string? MotherTongue { get; set; }
    public string? BloodGroup { get; set; }
    public string? AboutMe { get; set; }
    public bool IsProfileCompleted { get; set; }
    public string ProfileStatus { get; set; } = "Pending";
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedOn { get; set; }

    public User User { get; set; } = null!;
}
