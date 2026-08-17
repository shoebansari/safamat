namespace Matrimonial.AdminApi.DTOs.TenantSubscriptions;

public class TenantSubscriptionDto
{
    public Guid TenantSubscriptionsId { get; set; }
    public Guid TenantId { get; set; }
    public string? TenantName { get; set; }
    public Guid PlanId { get; set; }
    public string? PlanName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? NextBillingDate { get; set; }
    public decimal Amount { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public string SubscriptionStatus { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
}

public class CreateTenantSubscriptionRequest
{
    public Guid TenantId { get; set; }
    public Guid PlanId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? NextBillingDate { get; set; }
    public decimal Amount { get; set; }
    public string PaymentStatus { get; set; } = "Pending";
    public string SubscriptionStatus { get; set; } = "Active";
}

public class UpdateTenantSubscriptionRequest
{
    public DateTime? EndDate { get; set; }
    public DateTime? NextBillingDate { get; set; }
    public decimal? Amount { get; set; }
    public string? PaymentStatus { get; set; }
    public string? SubscriptionStatus { get; set; }
}
