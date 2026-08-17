namespace Matrimonial.AdminApi.DTOs.Payments;

public class PaymentDto
{
    public Guid PaymentId { get; set; }
    public Guid SubscriptionId { get; set; }
    public Guid TenantId { get; set; }
    public string? TenantName { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string? PaymentMethod { get; set; }
    public string? TransactionId { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? PaymentGateway { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? PaidOn { get; set; }
}

public class CreatePaymentRequest
{
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
}

public class UpdatePaymentRequest
{
    public string? PaymentMethod { get; set; }
    public string? TransactionId { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? PaymentGateway { get; set; }
    public string? Status { get; set; }
    public DateTime? PaidOn { get; set; }
}
