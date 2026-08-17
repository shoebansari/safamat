using Matrimonial.AdminApi.Common;
using Matrimonial.AdminApi.Data;
using Matrimonial.AdminApi.DTOs.Payments;
using Matrimonial.AdminApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matrimonial.AdminApi.Services;

public interface IPaymentService
{
    Task<PagedResult<PaymentDto>> GetAllAsync(int page, int pageSize, Guid? tenantId, string? status);
    Task<PaymentDto?> GetByIdAsync(Guid id);
    Task<PaymentDto> CreateAsync(CreatePaymentRequest request);
    Task<PaymentDto?> UpdateAsync(Guid id, UpdatePaymentRequest request);
    Task<bool> DeleteAsync(Guid id);
}

public class PaymentService : IPaymentService
{
    private readonly ApplicationDbContext _context;

    public PaymentService(ApplicationDbContext context) => _context = context;

    public async Task<PagedResult<PaymentDto>> GetAllAsync(int page, int pageSize, Guid? tenantId, string? status)
    {
        var query = _context.Payments.Include(p => p.Tenant).AsQueryable();

        if (tenantId.HasValue)
            query = query.Where(p => p.TenantId == tenantId.Value);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(p => p.Status == status);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(p => p.PaidOn ?? DateTime.MinValue)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PaymentDto
            {
                PaymentId = p.PaymentId,
                SubscriptionId = p.SubscriptionId,
                TenantId = p.TenantId,
                TenantName = p.Tenant.CompanyName,
                Amount = p.Amount,
                Currency = p.Currency,
                PaymentMethod = p.PaymentMethod,
                TransactionId = p.TransactionId,
                InvoiceNumber = p.InvoiceNumber,
                PaymentGateway = p.PaymentGateway,
                Status = p.Status,
                PaidOn = p.PaidOn
            })
            .ToListAsync();

        return new PagedResult<PaymentDto> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
    }

    public async Task<PaymentDto?> GetByIdAsync(Guid id)
    {
        var payment = await _context.Payments
            .Include(p => p.Tenant)
            .FirstOrDefaultAsync(p => p.PaymentId == id);

        if (payment == null) return null;

        return new PaymentDto
        {
            PaymentId = payment.PaymentId,
            SubscriptionId = payment.SubscriptionId,
            TenantId = payment.TenantId,
            TenantName = payment.Tenant.CompanyName,
            Amount = payment.Amount,
            Currency = payment.Currency,
            PaymentMethod = payment.PaymentMethod,
            TransactionId = payment.TransactionId,
            InvoiceNumber = payment.InvoiceNumber,
            PaymentGateway = payment.PaymentGateway,
            Status = payment.Status,
            PaidOn = payment.PaidOn
        };
    }

    public async Task<PaymentDto> CreateAsync(CreatePaymentRequest request)
    {
        if (!await _context.TenantSubscriptions.AnyAsync(s => s.TenantSubscriptionsId == request.SubscriptionId))
            throw new InvalidOperationException("Subscription not found.");

        if (!await _context.Tenants.AnyAsync(t => t.TenantId == request.TenantId))
            throw new InvalidOperationException("Tenant not found.");

        var payment = new Payment
        {
            PaymentId = Guid.NewGuid(),
            SubscriptionId = request.SubscriptionId,
            TenantId = request.TenantId,
            Amount = request.Amount,
            Currency = request.Currency,
            PaymentMethod = request.PaymentMethod,
            TransactionId = request.TransactionId,
            InvoiceNumber = request.InvoiceNumber,
            PaymentGateway = request.PaymentGateway,
            Status = request.Status,
            PaidOn = DateTimeHelper.ToUtcDate(request.PaidOn)
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        await UpdateSubscriptionPaymentStatusAsync(request.SubscriptionId);

        return (await GetByIdAsync(payment.PaymentId))!;
    }

    public async Task<PaymentDto?> UpdateAsync(Guid id, UpdatePaymentRequest request)
    {
        var payment = await _context.Payments.FindAsync(id);
        if (payment == null) return null;

        if (request.PaymentMethod != null) payment.PaymentMethod = request.PaymentMethod;
        if (request.TransactionId != null) payment.TransactionId = request.TransactionId;
        if (request.InvoiceNumber != null) payment.InvoiceNumber = request.InvoiceNumber;
        if (request.PaymentGateway != null) payment.PaymentGateway = request.PaymentGateway;
        if (request.Status != null) payment.Status = request.Status;
        if (request.PaidOn.HasValue) payment.PaidOn = DateTimeHelper.ToUtcDate(request.PaidOn);

        await _context.SaveChangesAsync();

        if (request.Status != null)
            await UpdateSubscriptionPaymentStatusAsync(payment.SubscriptionId);

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var payment = await _context.Payments.FindAsync(id);
        if (payment == null) return false;

        payment.Status = "Cancelled";
        await _context.SaveChangesAsync();

        await UpdateSubscriptionPaymentStatusAsync(payment.SubscriptionId);

        return true;
    }

    private static string MapPaymentStatusToSubscription(string paymentStatus) => paymentStatus switch
    {
        "Paid" or "Completed" => "Paid",
        "Failed" => "Failed",
        "Refunded" => "Refunded",
        "Pending" => "Pending",
        _ => "Pending"
    };

    private async Task UpdateSubscriptionPaymentStatusAsync(Guid subscriptionId)
    {
        var sub = await _context.TenantSubscriptions.FindAsync(subscriptionId);
        if (sub == null) return;

        var latestPayment = await _context.Payments
            .Where(p => p.SubscriptionId == subscriptionId && p.Status != "Cancelled")
            .OrderByDescending(p => p.PaidOn ?? DateTime.MinValue)
            .ThenByDescending(p => p.PaymentId)
            .FirstOrDefaultAsync();

        sub.PaymentStatus = latestPayment == null
            ? "Pending"
            : MapPaymentStatusToSubscription(latestPayment.Status);

        await _context.SaveChangesAsync();
    }
}
