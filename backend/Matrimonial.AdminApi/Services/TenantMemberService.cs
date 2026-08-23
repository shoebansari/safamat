using Matrimonial.AdminApi.Data;
using Matrimonial.AdminApi.DTOs.TenantPanel;
using Matrimonial.AdminApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matrimonial.AdminApi.Services;

public interface ITenantMemberService
{
    Task<MemberDto?> GetByUserCodeAsync(Guid tenantId, string userCode);
    Task<List<MemberDto>> GetPendingApprovalsAsync(Guid tenantId);
    Task<MemberDto?> UpdatePlanAssignmentAsync(Guid tenantId, string userCode, UpdateMemberPlanAssignmentRequest request);
    Task<MemberDto?> UpdateProfileApprovalAsync(Guid tenantId, Guid memberId, string status);
    Task<MemberDto?> UpdatePhotoApprovalAsync(Guid tenantId, Guid memberId, string status);
}

public class TenantMemberService : ITenantMemberService
{
    private readonly ApplicationDbContext _context;

    public TenantMemberService(ApplicationDbContext context) => _context = context;

    public async Task<MemberDto?> GetByUserCodeAsync(Guid tenantId, string userCode)
    {
        var member = await _context.Members
            .Include(m => m.Subscriptions)
            .ThenInclude(s => s.Plan)
            .FirstOrDefaultAsync(m => m.TenantId == tenantId && m.UserCode.ToLower() == userCode.ToLower());

        return member == null ? null : MapToDto(member);
    }

    public async Task<List<MemberDto>> GetPendingApprovalsAsync(Guid tenantId)
    {
        var members = await _context.Members
            .Include(m => m.Subscriptions)
            .ThenInclude(s => s.Plan)
            .Where(m => m.TenantId == tenantId &&
                        (m.ProfileStatus == "Pending" || m.PhotoStatus == "Pending"))
            .OrderByDescending(m => m.CreatedOn)
            .ToListAsync();
        return members.Select(MapToDto).ToList();
    }

    public async Task<MemberDto?> UpdatePlanAssignmentAsync(
        Guid tenantId, string userCode, UpdateMemberPlanAssignmentRequest request)
    {
        var member = await _context.Members
            .Include(m => m.Subscriptions)
            .ThenInclude(s => s.Plan)
            .FirstOrDefaultAsync(m => m.TenantId == tenantId && m.UserCode.ToLower() == userCode.ToLower());

        if (member == null) return null;

        var plan = await _context.MemberPlans
            .FirstOrDefaultAsync(p => p.MemberPlanId == request.MemberPlanId && p.TenantId == tenantId && p.IsActive);
        if (plan == null)
            throw new InvalidOperationException("Plan not found.");

        var status = NormalizePaymentStatus(request.PaymentStatus);
        var existing = member.Subscriptions.OrderByDescending(s => s.AssignedOn).FirstOrDefault();

        if (existing != null)
        {
            existing.MemberPlanId = plan.MemberPlanId;
            existing.PaymentStatus = status;
            existing.UpdatedOn = DateTime.UtcNow;
        }
        else
        {
            _context.MemberSubscriptions.Add(new MemberSubscription
            {
                MemberSubscriptionId = Guid.NewGuid(),
                TenantId = tenantId,
                MemberId = member.MemberId,
                MemberPlanId = plan.MemberPlanId,
                PaymentStatus = status,
                AssignedOn = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();
        return await GetByUserCodeAsync(tenantId, userCode);
    }

    public async Task<MemberDto?> UpdateProfileApprovalAsync(Guid tenantId, Guid memberId, string status)
    {
        var member = await _context.Members.FirstOrDefaultAsync(m => m.MemberId == memberId && m.TenantId == tenantId);
        if (member == null) return null;
        member.ProfileStatus = NormalizeApprovalStatus(status);
        await _context.SaveChangesAsync();
        return await GetByUserCodeAsync(tenantId, member.UserCode);
    }

    public async Task<MemberDto?> UpdatePhotoApprovalAsync(Guid tenantId, Guid memberId, string status)
    {
        var member = await _context.Members.FirstOrDefaultAsync(m => m.MemberId == memberId && m.TenantId == tenantId);
        if (member == null) return null;
        member.PhotoStatus = NormalizeApprovalStatus(status);
        await _context.SaveChangesAsync();
        return await GetByUserCodeAsync(tenantId, member.UserCode);
    }

    private static string NormalizePaymentStatus(string status) => status switch
    {
        "Paid" => "Paid",
        "Rejected" or "Reject" => "Rejected",
        _ => "Pending"
    };

    private static string NormalizeApprovalStatus(string status) => status switch
    {
        "Approved" or "Approve" => "Approved",
        "Rejected" or "Reject" => "Rejected",
        _ => "Pending"
    };

    private static MemberDto MapToDto(Member m)
    {
        var sub = m.Subscriptions?.OrderByDescending(s => s.AssignedOn).FirstOrDefault();
        return new MemberDto
        {
            MemberId = m.MemberId,
            UserCode = m.UserCode,
            FullName = m.FullName,
            Email = m.Email,
            Phone = m.Phone,
            Bio = m.Bio,
            ProfilePhotoUrl = m.ProfilePhotoUrl,
            ProfileStatus = m.ProfileStatus,
            PhotoStatus = m.PhotoStatus,
            IsActive = m.IsActive,
            CreatedOn = m.CreatedOn,
            CurrentSubscription = sub == null ? null : new MemberSubscriptionDto
            {
                MemberSubscriptionId = sub.MemberSubscriptionId,
                MemberPlanId = sub.MemberPlanId,
                PlanName = sub.Plan?.PlanName ?? "",
                PlanPrice = sub.Plan?.Price ?? 0,
                PaymentStatus = sub.PaymentStatus,
                AssignedOn = sub.AssignedOn
            }
        };
    }
}
