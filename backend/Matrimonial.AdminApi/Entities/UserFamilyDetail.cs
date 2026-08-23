namespace Matrimonial.AdminApi.Entities;

public class UserFamilyDetail
{
    public Guid FamilyId { get; set; }
    public Guid UserId { get; set; }
    public string? FamilyType { get; set; }
    public string? FamilyStatus { get; set; }
    public string? FatherName { get; set; }
    public string? FatherOccupation { get; set; }
    public string? MotherName { get; set; }
    public string? MotherOccupation { get; set; }
    public int? Brothers { get; set; }
    public int? Sisters { get; set; }

    public User User { get; set; } = null!;
}
