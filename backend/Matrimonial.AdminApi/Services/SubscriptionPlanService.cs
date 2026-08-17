using Matrimonial.AdminApi.Common;
using Matrimonial.AdminApi.Data;
using Matrimonial.AdminApi.DTOs.SubscriptionPlans;
using Matrimonial.AdminApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matrimonial.AdminApi.Services;

public interface ISubscriptionPlanService
{
    Task<PagedResult<SubscriptionPlanDto>> GetAllAsync(int page, int pageSize, bool? isActive);
    Task<SubscriptionPlanDto?> GetByIdAsync(Guid id);
    Task<SubscriptionPlanDto> CreateAsync(CreateSubscriptionPlanRequest request);
    Task<SubscriptionPlanDto?> UpdateAsync(Guid id, UpdateSubscriptionPlanRequest request);
    Task<bool> DeleteAsync(Guid id);
}

public class SubscriptionPlanService : ISubscriptionPlanService
{
    private readonly ApplicationDbContext _context;

    public SubscriptionPlanService(ApplicationDbContext context) => _context = context;

    public async Task<PagedResult<SubscriptionPlanDto>> GetAllAsync(int page, int pageSize, bool? isActive)
    {
        var query = _context.SubscriptionPlans.AsQueryable();

        if (isActive.HasValue)
            query = query.Where(p => p.IsActive == isActive.Value);

        var total = await query.CountAsync();
        var plans = await query
            .OrderByDescending(p => p.CreatedOn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        var items = plans.Select(MapToDto).ToList();

        return new PagedResult<SubscriptionPlanDto> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
    }

    public async Task<SubscriptionPlanDto?> GetByIdAsync(Guid id)
    {
        var plan = await _context.SubscriptionPlans.FindAsync(id);
        return plan == null ? null : MapToDto(plan);
    }

    public async Task<SubscriptionPlanDto> CreateAsync(CreateSubscriptionPlanRequest request)
    {
        var plan = new SubscriptionPlan
        {
            PlanId = Guid.NewGuid(),
            PlanName = request.PlanName,
            Description = request.Description,
            Price = request.Price,
            DurationDays = request.DurationDays,
            IsActive = request.IsActive,
            CreatedOn = DateTime.UtcNow
        };

        _context.SubscriptionPlans.Add(plan);
        await _context.SaveChangesAsync();
        return MapToDto(plan);
    }

    public async Task<SubscriptionPlanDto?> UpdateAsync(Guid id, UpdateSubscriptionPlanRequest request)
    {
        var plan = await _context.SubscriptionPlans.FindAsync(id);
        if (plan == null) return null;

        if (request.PlanName != null) plan.PlanName = request.PlanName;
        if (request.Description != null) plan.Description = request.Description;
        if (request.Price.HasValue) plan.Price = request.Price.Value;
        if (request.DurationDays.HasValue) plan.DurationDays = request.DurationDays.Value;
        if (request.IsActive.HasValue) plan.IsActive = request.IsActive.Value;

        await _context.SaveChangesAsync();
        return MapToDto(plan);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var plan = await _context.SubscriptionPlans.FindAsync(id);
        if (plan == null) return false;

        plan.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    private static SubscriptionPlanDto MapToDto(SubscriptionPlan p) => new()
    {
        PlanId = p.PlanId,
        PlanName = p.PlanName,
        Description = p.Description,
        Price = p.Price,
        DurationDays = p.DurationDays,
        IsActive = p.IsActive,
        CreatedOn = p.CreatedOn
    };
}
