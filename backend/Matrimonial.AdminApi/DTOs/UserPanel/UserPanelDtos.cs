namespace Matrimonial.AdminApi.DTOs.UserPanel;

using Matrimonial.AdminApi.DTOs.TenantPanel;

public class UserRegisterRequest
{
    public string TenantCode { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Password { get; set; } = string.Empty;
    public Guid MemberPlanId { get; set; }
}

public class ChangePlanRequest
{
    public Guid MemberPlanId { get; set; }
}

public class UserSubscriptionDto
{
    public Guid UserSubscriptionId { get; set; }
    public Guid MemberPlanId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public decimal PlanPrice { get; set; }
    public int DurationDays { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public DateTime AssignedOn { get; set; }
}

public class UserLoginRequest
{
    public string TenantCode { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class UserLoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserSessionDto User { get; set; } = null!;
}

public class UserSessionDto
{
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public string TenantCode { get; set; } = string.Empty;
    public string UserCode { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? PrimaryPhotoUrl { get; set; }
    public bool IsProfileCompleted { get; set; }
}

public class UserProfileDto
{
    public Guid ProfileId { get; set; }
    public Guid UserId { get; set; }
    public string UserCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public int? Age { get; set; }
    public decimal? Height { get; set; }
    public decimal? Weight { get; set; }
    public string? MaritalStatus { get; set; }
    public string? Religion { get; set; }
    public string? Caste { get; set; }
    public string? SubCaste { get; set; }
    public string? MotherTongue { get; set; }
    public string? BloodGroup { get; set; }
    public string? AboutMe { get; set; }
    public bool IsProfileCompleted { get; set; }
    public string ProfileStatus { get; set; } = string.Empty;
    public string? PrimaryPhotoUrl { get; set; }
    public UserEducationDto? Education { get; set; }
    public UserOccupationDto? Occupation { get; set; }
    public UserFamilyDto? Family { get; set; }
    public UserLifestyleDto? Lifestyle { get; set; }
    public UserLocationDto? Location { get; set; }
    public List<UserPhotoDto> Photos { get; set; } = new();
}

public class UserEducationDto
{
    public Guid EducationId { get; set; }
    public string? Qualification { get; set; }
    public string? College { get; set; }
    public string? University { get; set; }
    public int? PassingYear { get; set; }
    public string? EducationType { get; set; }
}

public class UserOccupationDto
{
    public Guid OccupationId { get; set; }
    public string? Occupation { get; set; }
    public string? CompanyName { get; set; }
    public string? Designation { get; set; }
    public decimal? AnnualIncome { get; set; }
    public string? WorkLocation { get; set; }
}

public class UserFamilyDto
{
    public Guid FamilyId { get; set; }
    public string? FamilyType { get; set; }
    public string? FamilyStatus { get; set; }
    public string? FatherName { get; set; }
    public string? FatherOccupation { get; set; }
    public string? MotherName { get; set; }
    public string? MotherOccupation { get; set; }
    public int? Brothers { get; set; }
    public int? Sisters { get; set; }
}

public class UserLifestyleDto
{
    public Guid LifestyleId { get; set; }
    public string? Diet { get; set; }
    public bool Smoking { get; set; }
    public bool Drinking { get; set; }
    public string? Hobbies { get; set; }
    public string? LanguagesKnown { get; set; }
}

public class UserLocationDto
{
    public Guid LocationId { get; set; }
    public string? Country { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }
    public string? Pincode { get; set; }
}

public class UserPhotoDto
{
    public Guid PhotoId { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsApproved { get; set; }
    public DateTime UploadedOn { get; set; }
}

public class SaveProfileRequest
{
    public string? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public decimal? Height { get; set; }
    public decimal? Weight { get; set; }
    public string? MaritalStatus { get; set; }
    public string? Religion { get; set; }
    public string? Caste { get; set; }
    public string? SubCaste { get; set; }
    public string? MotherTongue { get; set; }
    public string? BloodGroup { get; set; }
    public string? AboutMe { get; set; }
}

public class SaveEducationRequest
{
    public string? Qualification { get; set; }
    public string? College { get; set; }
    public string? University { get; set; }
    public int? PassingYear { get; set; }
    public string? EducationType { get; set; }
}

public class SaveOccupationRequest
{
    public string? Occupation { get; set; }
    public string? CompanyName { get; set; }
    public string? Designation { get; set; }
    public decimal? AnnualIncome { get; set; }
    public string? WorkLocation { get; set; }
}

public class SaveFamilyRequest
{
    public string? FamilyType { get; set; }
    public string? FamilyStatus { get; set; }
    public string? FatherName { get; set; }
    public string? FatherOccupation { get; set; }
    public string? MotherName { get; set; }
    public string? MotherOccupation { get; set; }
    public int? Brothers { get; set; }
    public int? Sisters { get; set; }
}

public class SaveLifestyleRequest
{
    public string? Diet { get; set; }
    public bool Smoking { get; set; }
    public bool Drinking { get; set; }
    public string? Hobbies { get; set; }
    public string? LanguagesKnown { get; set; }
}

public class SaveLocationRequest
{
    public string? Country { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }
    public string? Pincode { get; set; }
}

public class SavePreferenceRequest
{
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public decimal? MinHeight { get; set; }
    public decimal? MaxHeight { get; set; }
    public string? Religion { get; set; }
    public string? Caste { get; set; }
    public string? Education { get; set; }
    public string? Occupation { get; set; }
    public string? Country { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
}

public class AddPhotoRequest
{
    public string PhotoUrl { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}

public class UserPreferenceDto : SavePreferenceRequest
{
    public Guid PreferenceId { get; set; }
}

public class DiscoverFilterRequest
{
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Religion { get; set; }
    public string? Caste { get; set; }
    public string? MotherTongue { get; set; }
    public string? MaritalStatus { get; set; }
    public string? Occupation { get; set; }
    public string? Education { get; set; }
    public string? Gender { get; set; }
    public decimal? MinHeight { get; set; }
    public decimal? MaxHeight { get; set; }
}

public class DiscoverFilterOptionsDto
{
    public List<string> Cities { get; set; } = [];
    public List<string> States { get; set; } = [];
    public List<string> Religions { get; set; } = [];
    public List<string> MotherTongues { get; set; } = [];
    public List<string> MaritalStatuses { get; set; } = [];
    public List<string> Occupations { get; set; } = [];
    public List<string> Educations { get; set; } = [];
    public List<string> Genders { get; set; } = [];
}

public class DiscoverProfileDto
{
    public Guid UserId { get; set; }
    public string UserCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int? Age { get; set; }
    public string? Gender { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Religion { get; set; }
    public string? Caste { get; set; }
    public string? MotherTongue { get; set; }
    public string? MaritalStatus { get; set; }
    public string? Occupation { get; set; }
    public string? Education { get; set; }
    public decimal? Height { get; set; }
    public string? PrimaryPhotoUrl { get; set; }
    public List<string> PhotoUrls { get; set; } = [];
    public decimal MatchPercentage { get; set; }
}

public class InterestRequestDto
{
    public Guid InterestId { get; set; }
    public Guid SenderUserId { get; set; }
    public Guid ReceiverUserId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string ReceiverName { get; set; } = string.Empty;
    public string? SenderPhotoUrl { get; set; }
    public string? ReceiverPhotoUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime SentOn { get; set; }
}

public class MatchDto
{
    public Guid MatchId { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public decimal MatchPercentage { get; set; }
    public DateTime MatchedOn { get; set; }
}

public class MessageDto
{
    public Guid MessageId { get; set; }
    public Guid SenderUserId { get; set; }
    public Guid ReceiverUserId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime SentOn { get; set; }
}

public class ConversationDto
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public string? LastMessage { get; set; }
    public DateTime? LastMessageOn { get; set; }
    public int UnreadCount { get; set; }
}

public class NotificationDto
{
    public Guid NotificationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Guid? RelatedUserId { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedOn { get; set; }
}

public class SendMessageRequest
{
    public Guid ReceiverUserId { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class ReportUserRequest
{
    public Guid ReportedUserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Details { get; set; }
}

// Tenant panel - user approval DTOs
public class TenantUserDto
{
    public Guid UserId { get; set; }
    public string UserCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? AboutMe { get; set; }
    public string ProfileStatus { get; set; } = string.Empty;
    public string? PrimaryPhotoUrl { get; set; }
    public string? PendingPhotoUrl { get; set; }
    public List<TenantUserPhotoDto> Photos { get; set; } = [];
    public bool HasPendingPhoto { get; set; }
    public Guid? PendingPhotoId { get; set; }
    public DateTime CreatedOn { get; set; }
    public MemberSubscriptionDto? CurrentSubscription { get; set; }
}

public class TenantUserPhotoDto
{
    public Guid PhotoId { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public bool IsPrimary { get; set; }
}
