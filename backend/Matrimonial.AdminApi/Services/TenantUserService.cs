using Matrimonial.AdminApi.Data;
using Matrimonial.AdminApi.DTOs.TenantPanel;
using Matrimonial.AdminApi.DTOs.UserPanel;
using Microsoft.EntityFrameworkCore;

namespace Matrimonial.AdminApi.Services;

public interface ITenantUserService
{
    Task<TenantUserDto?> GetByUserCodeAsync(Guid tenantId, string userCode);
    Task<List<TenantUserDto>> GetPendingApprovalsAsync(Guid tenantId);
    Task<TenantUserDto?> UpdatePlanAssignmentAsync(Guid tenantId, string userCode, UpdateMemberPlanAssignmentRequest request);
    Task<TenantUserDto?> UpdateProfileApprovalAsync(Guid tenantId, Guid userId, string status);
    Task<TenantUserDto?> UpdatePhotoApprovalAsync(Guid tenantId, Guid photoId, string status);
    Task<UserProfileDto?> GetUserProfileDetailAsync(Guid tenantId, Guid userId);
}

public class TenantUserService : ITenantUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IUserProfileService _profiles;

    public TenantUserService(ApplicationDbContext context, IUserProfileService profiles)
    {
        _context = context;
        _profiles = profiles;
    }

    public async Task<TenantUserDto?> GetByUserCodeAsync(Guid tenantId, string userCode)
    {
        var user = await LoadUserAsync(tenantId, userCode);
        return user == null ? null : MapToDto(user);
    }

    public async Task<List<TenantUserDto>> GetPendingApprovalsAsync(Guid tenantId)
    {
        var users = await _context.Users
            .Include(u => u.Profile)
            .Include(u => u.Photos)
            .Include(u => u.Subscriptions).ThenInclude(s => s.Plan)
            .Where(u => u.TenantId == tenantId &&
                        ((u.Profile != null && u.Profile.ProfileStatus == "Pending") ||
                         u.Photos.Any(p => !p.IsApproved) ||
                         u.Subscriptions.Any(s => s.PaymentStatus == "Pending")))
            .OrderByDescending(u => u.CreatedOn)
            .ToListAsync();
        return users.Select(MapToDto).ToList();
    }

    public async Task<TenantUserDto?> UpdatePlanAssignmentAsync(
        Guid tenantId, string userCode, UpdateMemberPlanAssignmentRequest request)
    {
        var user = await LoadUserAsync(tenantId, userCode);
        if (user == null) return null;

        var plan = await _context.MemberPlans.FirstOrDefaultAsync(p =>
            p.MemberPlanId == request.MemberPlanId && p.TenantId == tenantId && p.IsActive);
        if (plan == null) throw new InvalidOperationException("Plan not found.");

        var status = request.PaymentStatus switch
        {
            "Paid" => "Paid",
            "Rejected" or "Reject" => "Rejected",
            _ => "Pending"
        };

        var existing = user.Subscriptions.OrderByDescending(s => s.AssignedOn).FirstOrDefault();
        if (existing != null)
        {
            existing.MemberPlanId = plan.MemberPlanId;
            existing.PaymentStatus = status;
            existing.UpdatedOn = DateTime.UtcNow;
        }
        else
        {
            _context.UserSubscriptions.Add(new Entities.UserSubscription
            {
                UserSubscriptionId = Guid.NewGuid(),
                TenantId = tenantId,
                UserId = user.UserId,
                MemberPlanId = plan.MemberPlanId,
                PaymentStatus = status,
                AssignedOn = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();

        _context.Notifications.Add(new Entities.Notification
        {
            NotificationId = Guid.NewGuid(),
            UserId = user.UserId,
            Title = status == "Paid" ? "Plan Approved" : status == "Rejected" ? "Plan Rejected" : "Plan Updated",
            MessageText = status == "Paid"
                ? $"Your {plan.PlanName} plan has been approved."
                : status == "Rejected"
                    ? $"Your {plan.PlanName} plan request was rejected. Please contact your tenant."
                    : $"Your plan has been updated to {plan.PlanName} (pending approval).",
            CreatedOn = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        return await GetByUserCodeAsync(tenantId, userCode);
    }

    public async Task<TenantUserDto?> UpdateProfileApprovalAsync(Guid tenantId, Guid userId, string status)
    {
        var user = await _context.Users.Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.UserId == userId && u.TenantId == tenantId);
        if (user?.Profile == null) return null;

        user.Profile.ProfileStatus = status switch
        {
            "Approved" or "Approve" => "Approved",
            "Rejected" or "Reject" => "Rejected",
            _ => "Pending"
        };
        user.Profile.UpdatedOn = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return await GetByUserCodeAsync(tenantId, user.UserCode);
    }

    public async Task<TenantUserDto?> UpdatePhotoApprovalAsync(Guid tenantId, Guid photoId, string status)
    {
        var photo = await _context.UserPhotos
            .Include(p => p.User).ThenInclude(u => u.Profile)
            .Include(p => p.User).ThenInclude(u => u.Photos)
            .Include(p => p.User).ThenInclude(u => u.Subscriptions).ThenInclude(s => s.Plan)
            .FirstOrDefaultAsync(p => p.PhotoId == photoId && p.User.TenantId == tenantId);
        if (photo == null) return null;

        photo.IsApproved = status is "Approved" or "Approve";
        await _context.SaveChangesAsync();
        return MapToDto(photo.User);
    }

    public Task<UserProfileDto?> GetUserProfileDetailAsync(Guid tenantId, Guid userId) =>
        _profiles.GetForTenantReviewAsync(tenantId, userId);

    private async Task<Entities.User?> LoadUserAsync(Guid tenantId, string userCode) =>
        await _context.Users
            .Include(u => u.Profile)
            .Include(u => u.Photos)
            .Include(u => u.Subscriptions).ThenInclude(s => s.Plan)
            .FirstOrDefaultAsync(u =>
                u.TenantId == tenantId &&
                (u.UserCode.ToLower() == userCode.ToLower() || u.UserName.ToLower() == userCode.ToLower()));

    private static TenantUserDto MapToDto(Entities.User u)
    {
        var sub = u.Subscriptions?.OrderByDescending(s => s.AssignedOn).FirstOrDefault();
        var pendingPhoto = u.Photos.FirstOrDefault(p => !p.IsApproved);
        return new TenantUserDto
        {
            UserId = u.UserId,
            UserCode = u.UserCode,
            FullName = $"{u.FirstName} {u.LastName}",
            Email = u.Email,
            Phone = u.Phone,
            AboutMe = u.Profile?.AboutMe,
            ProfileStatus = u.Profile?.ProfileStatus ?? "Pending",
            PrimaryPhotoUrl = u.Photos.FirstOrDefault(p => p.IsApproved && p.IsPrimary)?.PhotoUrl
                ?? u.Photos.FirstOrDefault(p => p.IsApproved)?.PhotoUrl
                ?? pendingPhoto?.PhotoUrl,
            PendingPhotoUrl = pendingPhoto?.PhotoUrl,
            Photos = u.Photos.OrderBy(p => p.DisplayOrder).Select(p => new TenantUserPhotoDto
            {
                PhotoId = p.PhotoId,
                PhotoUrl = p.PhotoUrl,
                IsApproved = p.IsApproved,
                IsPrimary = p.IsPrimary
            }).ToList(),
            HasPendingPhoto = u.Photos.Any(p => !p.IsApproved),
            PendingPhotoId = u.Photos.FirstOrDefault(p => !p.IsApproved)?.PhotoId,
            CreatedOn = u.CreatedOn,
            CurrentSubscription = sub == null ? null : new MemberSubscriptionDto
            {
                MemberSubscriptionId = sub.UserSubscriptionId,
                MemberPlanId = sub.MemberPlanId,
                PlanName = sub.Plan?.PlanName ?? "",
                PlanPrice = sub.Plan?.Price ?? 0,
                PaymentStatus = sub.PaymentStatus,
                AssignedOn = sub.AssignedOn
            }
        };
    }
}
