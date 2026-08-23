namespace Matrimonial.AdminApi.Entities;

public class UserLifestyle
{
    public Guid LifestyleId { get; set; }
    public Guid UserId { get; set; }
    public string? Diet { get; set; }
    public bool Smoking { get; set; }
    public bool Drinking { get; set; }
    public string? Hobbies { get; set; }
    public string? LanguagesKnown { get; set; }

    public User User { get; set; } = null!;
}
