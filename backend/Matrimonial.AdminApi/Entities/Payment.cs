namespace Matrimonial.AdminApi.Entities;

public class Payment
{
    public Guid PaymentId { get; set; }
    public Guid SubscriptionId { get; set; }
    public Guid TenantId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
    public string? PaymentMethod { get; set; }
    public string? TransactionId { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? PaymentGateway { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime? PaidOn { get; set; }

    public TenantSubscription Subscription { get; set; } = null!;
    public Tenant Tenant { get; set; } = null!;
}
