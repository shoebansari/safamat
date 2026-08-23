namespace Matrimonial.AdminApi.DTOs.TenantPanel;

public class TenantLoginRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class TenantLoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public TenantSessionInfo Tenant { get; set; } = null!;
}

public class TenantSessionInfo
{
    public Guid TenantId { get; set; }
    public string TenantCode { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class MemberPlanDto
{
    public Guid MemberPlanId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DurationDays { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
}

public class CreateMemberPlanRequest
{
    public string PlanName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DurationDays { get; set; } = 30;
    public bool IsActive { get; set; } = true;
}

public class UpdateMemberPlanRequest
{
    public string? PlanName { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int? DurationDays { get; set; }
    public bool? IsActive { get; set; }
}

public class MemberDto
{
    public Guid MemberId { get; set; }
    public string UserCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public string ProfileStatus { get; set; } = string.Empty;
    public string PhotoStatus { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
    public MemberSubscriptionDto? CurrentSubscription { get; set; }
}

public class MemberSubscriptionDto
{
    public Guid MemberSubscriptionId { get; set; }
    public Guid MemberPlanId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public decimal PlanPrice { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public DateTime AssignedOn { get; set; }
}

public class UpdateMemberPlanAssignmentRequest
{
    public Guid MemberPlanId { get; set; }
    public string PaymentStatus { get; set; } = "Pending";
}

public class UpdateApprovalRequest
{
    public string Status { get; set; } = "Approved";
}
