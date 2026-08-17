using Matrimonial.AdminApi.Common;
using Matrimonial.AdminApi.Data;
using Matrimonial.AdminApi.DTOs.TenantSubscriptions;
using Matrimonial.AdminApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matrimonial.AdminApi.Services;

public interface ITenantSubscriptionService
{
    Task<PagedResult<TenantSubscriptionDto>> GetAllAsync(int page, int pageSize, Guid? tenantId, string? status);
    Task<TenantSubscriptionDto?> GetByIdAsync(Guid id);
    Task<TenantSubscriptionDto> CreateAsync(CreateTenantSubscriptionRequest request);
    Task<TenantSubscriptionDto?> UpdateAsync(Guid id, UpdateTenantSubscriptionRequest request);
    Task<bool> DeleteAsync(Guid id);
}

public class TenantSubscriptionService : ITenantSubscriptionService
{
    private readonly ApplicationDbContext _context;

    public TenantSubscriptionService(ApplicationDbContext context) => _context = context;

    public async Task<PagedResult<TenantSubscriptionDto>> GetAllAsync(int page, int pageSize, Guid? tenantId, string? status)
    {
        var query = _context.TenantSubscriptions
            .Include(s => s.Tenant)
            .Include(s => s.Plan)
            .AsQueryable();

        if (tenantId.HasValue)
            query = query.Where(s => s.TenantId == tenantId.Value);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(s => s.SubscriptionStatus == status);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(s => s.CreatedOn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new TenantSubscriptionDto
            {
                TenantSubscriptionsId = s.TenantSubscriptionsId,
                TenantId = s.TenantId,
                TenantName = s.Tenant.CompanyName,
                PlanId = s.PlanId,
                PlanName = s.Plan.PlanName,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                NextBillingDate = s.NextBillingDate,
                Amount = s.Amount,
                PaymentStatus = s.PaymentStatus,
                SubscriptionStatus = s.SubscriptionStatus,
                CreatedOn = s.CreatedOn
            })
            .ToListAsync();

        return new PagedResult<TenantSubscriptionDto> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
    }

    public async Task<TenantSubscriptionDto?> GetByIdAsync(Guid id)
    {
        var sub = await _context.TenantSubscriptions
            .Include(s => s.Tenant)
            .Include(s => s.Plan)
            .FirstOrDefaultAsync(s => s.TenantSubscriptionsId == id);

        if (sub == null) return null;

        return new TenantSubscriptionDto
        {
            TenantSubscriptionsId = sub.TenantSubscriptionsId,
            TenantId = sub.TenantId,
            TenantName = sub.Tenant.CompanyName,
            PlanId = sub.PlanId,
            PlanName = sub.Plan.PlanName,
            StartDate = sub.StartDate,
            EndDate = sub.EndDate,
            NextBillingDate = sub.NextBillingDate,
            Amount = sub.Amount,
            PaymentStatus = sub.PaymentStatus,
            SubscriptionStatus = sub.SubscriptionStatus,
            CreatedOn = sub.CreatedOn
        };
    }

    public async Task<TenantSubscriptionDto> CreateAsync(CreateTenantSubscriptionRequest request)
    {
        if (!await _context.Tenants.AnyAsync(t => t.TenantId == request.TenantId))
            throw new InvalidOperationException("Tenant not found.");

        if (!await _context.SubscriptionPlans.AnyAsync(p => p.PlanId == request.PlanId))
            throw new InvalidOperationException("Subscription plan not found.");

        var subscription = new TenantSubscription
        {
            TenantSubscriptionsId = Guid.NewGuid(),
            TenantId = request.TenantId,
            PlanId = request.PlanId,
            StartDate = DateTimeHelper.ToUtcDate(request.StartDate),
            EndDate = DateTimeHelper.ToUtcDate(request.EndDate),
            NextBillingDate = DateTimeHelper.ToUtcDate(request.NextBillingDate),
            Amount = request.Amount,
            PaymentStatus = request.PaymentStatus,
            SubscriptionStatus = request.SubscriptionStatus,
            CreatedOn = DateTime.UtcNow
        };

        _context.TenantSubscriptions.Add(subscription);
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(subscription.TenantSubscriptionsId))!;
    }

    public async Task<TenantSubscriptionDto?> UpdateAsync(Guid id, UpdateTenantSubscriptionRequest request)
    {
        var sub = await _context.TenantSubscriptions.FindAsync(id);
        if (sub == null) return null;

        if (request.EndDate.HasValue) sub.EndDate = DateTimeHelper.ToUtcDate(request.EndDate.Value);
        if (request.NextBillingDate.HasValue) sub.NextBillingDate = DateTimeHelper.ToUtcDate(request.NextBillingDate);
        if (request.Amount.HasValue) sub.Amount = request.Amount.Value;
        if (request.PaymentStatus != null) sub.PaymentStatus = request.PaymentStatus;
        if (request.SubscriptionStatus != null) sub.SubscriptionStatus = request.SubscriptionStatus;

        await _context.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var sub = await _context.TenantSubscriptions.FindAsync(id);
        if (sub == null) return false;

        sub.SubscriptionStatus = "Inactive";
        await _context.SaveChangesAsync();
        return true;
    }
}
