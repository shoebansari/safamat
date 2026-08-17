namespace Matrimonial.AdminApi.Entities;

public class TenantSubscription
{
    public Guid TenantSubscriptionsId { get; set; }
    public Guid TenantId { get; set; }
    public Guid PlanId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? NextBillingDate { get; set; }
    public decimal Amount { get; set; }
    public string PaymentStatus { get; set; } = "Pending";
    public string SubscriptionStatus { get; set; } = "Active";
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public Tenant Tenant { get; set; } = null!;
    public SubscriptionPlan Plan { get; set; } = null!;
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
