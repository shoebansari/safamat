namespace Matrimonial.AdminApi.Entities;

public class MemberSubscription
{
    public Guid MemberSubscriptionId { get; set; }
    public Guid TenantId { get; set; }
    public Guid MemberId { get; set; }
    public Guid MemberPlanId { get; set; }
    public string PaymentStatus { get; set; } = "Pending";
    public DateTime AssignedOn { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedOn { get; set; }

    public Tenant Tenant { get; set; } = null!;
    public Member Member { get; set; } = null!;
    public MemberPlan Plan { get; set; } = null!;
}
