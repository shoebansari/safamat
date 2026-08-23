namespace Matrimonial.AdminApi.Entities;

public class UserPreference
{
    public Guid PreferenceId { get; set; }
    public Guid UserId { get; set; }
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public decimal? MinHeight { get; set; }
    public decimal? MaxHeight { get; set; }
    public string? Religion { get; set; }
    public string? Caste { get; set; }
    public string? Education { get; set; }
    public string? Occupation { get; set; }
    public string? Country { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }

    public User User { get; set; } = null!;
}
