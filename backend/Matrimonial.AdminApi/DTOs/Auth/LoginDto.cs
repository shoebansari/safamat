namespace Matrimonial.AdminApi.DTOs.Auth;

public class LoginRequest
{
    public string AdminUserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public AdminUserInfo Admin { get; set; } = null!;
}

public class AdminUserInfo
{
    public Guid AdminId { get; set; }
    public string AdminUserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
