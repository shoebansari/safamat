using Matrimonial.AdminApi.Common;
using Matrimonial.AdminApi.Data;
using Matrimonial.AdminApi.DTOs.TenantPanel;
using Matrimonial.AdminApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matrimonial.AdminApi.Services;

public interface ITenantMemberPlanService
{
    Task<List<MemberPlanDto>> GetAllAsync(Guid tenantId);
    Task<MemberPlanDto> CreateAsync(Guid tenantId, CreateMemberPlanRequest request);
    Task<MemberPlanDto?> UpdateAsync(Guid tenantId, Guid planId, UpdateMemberPlanRequest request);
    Task<bool> DeleteAsync(Guid tenantId, Guid planId);
}

public class TenantMemberPlanService : ITenantMemberPlanService
{
    private readonly ApplicationDbContext _context;

    public TenantMemberPlanService(ApplicationDbContext context) => _context = context;

    public async Task<List<MemberPlanDto>> GetAllAsync(Guid tenantId)
    {
        var plans = await _context.MemberPlans
            .Where(p => p.TenantId == tenantId)
            .OrderByDescending(p => p.CreatedOn)
            .ToListAsync();
        return plans.Select(MapToDto).ToList();
    }

    public async Task<MemberPlanDto> CreateAsync(Guid tenantId, CreateMemberPlanRequest request)
    {
        var plan = new MemberPlan
        {
            MemberPlanId = Guid.NewGuid(),
            TenantId = tenantId,
            PlanName = request.PlanName,
            Description = request.Description,
            Price = request.Price,
            DurationDays = request.DurationDays,
            IsActive = request.IsActive,
            CreatedOn = DateTime.UtcNow
        };
        _context.MemberPlans.Add(plan);
        await _context.SaveChangesAsync();
        return MapToDto(plan);
    }

    public async Task<MemberPlanDto?> UpdateAsync(Guid tenantId, Guid planId, UpdateMemberPlanRequest request)
    {
        var plan = await _context.MemberPlans.FirstOrDefaultAsync(p => p.MemberPlanId == planId && p.TenantId == tenantId);
        if (plan == null) return null;

        if (request.PlanName != null) plan.PlanName = request.PlanName;
        if (request.Description != null) plan.Description = request.Description;
        if (request.Price.HasValue) plan.Price = request.Price.Value;
        if (request.DurationDays.HasValue) plan.DurationDays = request.DurationDays.Value;
        if (request.IsActive.HasValue) plan.IsActive = request.IsActive.Value;

        await _context.SaveChangesAsync();
        return MapToDto(plan);
    }

    public async Task<bool> DeleteAsync(Guid tenantId, Guid planId)
    {
        var plan = await _context.MemberPlans.FirstOrDefaultAsync(p => p.MemberPlanId == planId && p.TenantId == tenantId);
        if (plan == null) return false;
        plan.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    private static MemberPlanDto MapToDto(MemberPlan p) => new()
    {
        MemberPlanId = p.MemberPlanId,
        PlanName = p.PlanName,
        Description = p.Description,
        Price = p.Price,
        DurationDays = p.DurationDays,
        IsActive = p.IsActive,
        CreatedOn = p.CreatedOn
    };
}
