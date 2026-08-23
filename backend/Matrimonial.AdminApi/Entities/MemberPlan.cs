namespace Matrimonial.AdminApi.Entities;

public class MemberPlan
{
    public Guid MemberPlanId { get; set; }
    public Guid TenantId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DurationDays { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public Tenant Tenant { get; set; } = null!;
    public ICollection<MemberSubscription> Subscriptions { get; set; } = new List<MemberSubscription>();
}
