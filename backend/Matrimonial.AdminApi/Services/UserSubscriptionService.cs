using Matrimonial.AdminApi.Data;
using Matrimonial.AdminApi.DTOs.TenantPanel;
using Matrimonial.AdminApi.DTOs.UserPanel;
using Microsoft.EntityFrameworkCore;

namespace Matrimonial.AdminApi.Services;

public interface IUserSubscriptionService
{
    Task<List<MemberPlanDto>> GetPlansByTenantCodeAsync(string tenantCode);
    Task<List<MemberPlanDto>> GetPlansForUserAsync(Guid tenantId);
    Task<UserSubscriptionDto?> GetMySubscriptionAsync(Guid userId);
    Task<UserSubscriptionDto> RequestPlanChangeAsync(Guid userId, Guid tenantId, Guid memberPlanId);
}

public class UserSubscriptionService : IUserSubscriptionService
{
    private readonly ApplicationDbContext _context;

    public UserSubscriptionService(ApplicationDbContext context) => _context = context;

    public async Task<List<MemberPlanDto>> GetPlansByTenantCodeAsync(string tenantCode)
    {
        var tenant = await _context.Tenants.FirstOrDefaultAsync(t =>
            t.TenantCode.ToLower() == tenantCode.ToLower() && t.IsActive);
        if (tenant == null) return [];

        return await GetActivePlansAsync(tenant.TenantId);
    }

    public async Task<List<MemberPlanDto>> GetPlansForUserAsync(Guid tenantId) =>
        await GetActivePlansAsync(tenantId);

    public async Task<UserSubscriptionDto?> GetMySubscriptionAsync(Guid userId)
    {
        var sub = await _context.UserSubscriptions
            .Include(s => s.Plan)
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.AssignedOn)
            .FirstOrDefaultAsync();

        return sub == null ? null : MapSubscription(sub);
    }

    public async Task<UserSubscriptionDto> RequestPlanChangeAsync(Guid userId, Guid tenantId, Guid memberPlanId)
    {
        var plan = await _context.MemberPlans.FirstOrDefaultAsync(p =>
            p.MemberPlanId == memberPlanId && p.TenantId == tenantId && p.IsActive);
        if (plan == null) throw new InvalidOperationException("Selected plan is not available.");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId && u.TenantId == tenantId);
        if (user == null) throw new InvalidOperationException("User not found.");

        var existing = await _context.UserSubscriptions
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.AssignedOn)
            .FirstOrDefaultAsync();

        if (existing != null && existing.MemberPlanId == memberPlanId && existing.PaymentStatus == "Pending")
            throw new InvalidOperationException("This plan is already pending approval.");

        if (existing != null && existing.MemberPlanId == memberPlanId && existing.PaymentStatus == "Paid")
            throw new InvalidOperationException("You are already on this plan.");

        if (existing != null)
        {
            existing.MemberPlanId = plan.MemberPlanId;
            existing.PaymentStatus = "Pending";
            existing.UpdatedOn = DateTime.UtcNow;
        }
        else
        {
            _context.UserSubscriptions.Add(new Entities.UserSubscription
            {
                UserSubscriptionId = Guid.NewGuid(),
                TenantId = tenantId,
                UserId = userId,
                MemberPlanId = plan.MemberPlanId,
                PaymentStatus = "Pending",
                AssignedOn = DateTime.UtcNow
            });
        }

        _context.Notifications.Add(new Entities.Notification
        {
            NotificationId = Guid.NewGuid(),
            UserId = userId,
            Title = "Plan Change Requested",
            MessageText = $"Your request for the {plan.PlanName} plan has been submitted. Waiting for tenant approval.",
            CreatedOn = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        var updated = await _context.UserSubscriptions
            .Include(s => s.Plan)
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.AssignedOn)
            .FirstAsync();

        return MapSubscription(updated);
    }

    private async Task<List<MemberPlanDto>> GetActivePlansAsync(Guid tenantId) =>
        await _context.MemberPlans
            .Where(p => p.TenantId == tenantId && p.IsActive)
            .OrderBy(p => p.Price)
            .Select(p => new MemberPlanDto
            {
                MemberPlanId = p.MemberPlanId,
                PlanName = p.PlanName,
                Description = p.Description,
                Price = p.Price,
                DurationDays = p.DurationDays,
                IsActive = p.IsActive,
                CreatedOn = p.CreatedOn
            })
            .ToListAsync();

    private static UserSubscriptionDto MapSubscription(Entities.UserSubscription sub) => new()
    {
        UserSubscriptionId = sub.UserSubscriptionId,
        MemberPlanId = sub.MemberPlanId,
        PlanName = sub.Plan?.PlanName ?? "",
        PlanPrice = sub.Plan?.Price ?? 0,
        DurationDays = sub.Plan?.DurationDays ?? 0,
        PaymentStatus = sub.PaymentStatus,
        AssignedOn = sub.AssignedOn
    };
}
