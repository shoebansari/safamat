using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Matrimonial.AdminApi.Configurations;
using Matrimonial.AdminApi.Data;
using Matrimonial.AdminApi.DTOs.UserPanel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Matrimonial.AdminApi.Services;

public interface IUserAuthService
{
    Task<(UserLoginResponse? Result, string? Error)> RegisterAsync(UserRegisterRequest request);
    Task<(UserLoginResponse? Result, string? Error)> LoginAsync(UserLoginRequest request);
}

public class UserAuthService : IUserAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly JwtSettings _jwtSettings;

    public UserAuthService(ApplicationDbContext context, IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<(UserLoginResponse? Result, string? Error)> RegisterAsync(UserRegisterRequest request)
    {
        var tenant = await _context.Tenants.FirstOrDefaultAsync(t =>
            t.TenantCode.ToLower() == request.TenantCode.ToLower() && t.IsActive);
        if (tenant == null) return (null, "Invalid tenant code.");

        if (await _context.Users.AnyAsync(u =>
            u.TenantId == tenant.TenantId && u.UserName.ToLower() == request.UserName.ToLower()))
            return (null, "Username already exists.");

        if (await _context.Users.AnyAsync(u =>
            u.TenantId == tenant.TenantId && u.Email.ToLower() == request.Email.ToLower()))
            return (null, "Email already registered.");

        var plan = await _context.MemberPlans.FirstOrDefaultAsync(p =>
            p.MemberPlanId == request.MemberPlanId && p.TenantId == tenant.TenantId && p.IsActive);
        if (plan == null) return (null, "Please select a valid membership plan.");

        var userCode = await GenerateUserCodeAsync(tenant.TenantId);
        var userId = Guid.NewGuid();
        var user = new Entities.User
        {
            UserId = userId,
            TenantId = tenant.TenantId,
            UserCode = userCode,
            UserName = request.UserName,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            IsActive = true,
            CreatedOn = DateTime.UtcNow,
            Profile = new Entities.UserProfile
            {
                ProfileId = Guid.NewGuid(),
                UserId = userId,
                ProfileStatus = "Pending",
                CreatedOn = DateTime.UtcNow
            }
        };

        _context.Users.Add(user);
        _context.UserSubscriptions.Add(new Entities.UserSubscription
        {
            UserSubscriptionId = Guid.NewGuid(),
            TenantId = tenant.TenantId,
            UserId = userId,
            MemberPlanId = plan.MemberPlanId,
            PaymentStatus = "Pending",
            AssignedOn = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        return await BuildLoginResponseAsync(user, tenant);
    }

    public async Task<(UserLoginResponse? Result, string? Error)> LoginAsync(UserLoginRequest request)
    {
        var tenant = await _context.Tenants.FirstOrDefaultAsync(t =>
            t.TenantCode.ToLower() == request.TenantCode.ToLower() && t.IsActive);
        if (tenant == null) return (null, "Invalid tenant code.");

        var user = await _context.Users
            .Include(u => u.Profile)
            .Include(u => u.Photos)
            .FirstOrDefaultAsync(u =>
                u.TenantId == tenant.TenantId &&
                u.UserName.ToLower() == request.UserName.ToLower());

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            return (null, "Invalid username or password.");

        if (!user.IsActive)
            return (null, "Your account is inactive. Please contact support.");

        user.LastLogin = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return await BuildLoginResponseAsync(user, tenant);
    }

    private async Task<(UserLoginResponse? Result, string? Error)> BuildLoginResponseAsync(
        Entities.User user, Entities.Tenant tenant)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);
        var token = GenerateToken(user, tenant, expiresAt);
        var primaryPhoto = user.Photos?.FirstOrDefault(p => p.IsPrimary && p.IsApproved)?.PhotoUrl
            ?? user.Photos?.FirstOrDefault(p => p.IsApproved)?.PhotoUrl;

        return (new UserLoginResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = new UserSessionDto
            {
                UserId = user.UserId,
                TenantId = tenant.TenantId,
                TenantCode = tenant.TenantCode,
                UserCode = user.UserCode,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                PrimaryPhotoUrl = primaryPhoto,
                IsProfileCompleted = user.Profile?.IsProfileCompleted ?? false
            }
        }, null);
    }

    private string GenerateToken(Entities.User user, Entities.Tenant tenant, DateTime expiresAt)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim("user_id", user.UserId.ToString()),
            new Claim("tenant_id", tenant.TenantId.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, "User")
        };

        var token = new JwtSecurityToken(
            _jwtSettings.Issuer, _jwtSettings.Audience, claims, expires: expiresAt, signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<string> GenerateUserCodeAsync(Guid tenantId)
    {
        for (var i = 0; i < 100; i++)
        {
            var code = $"USR{Random.Shared.Next(100000, 999999)}";
            if (!await _context.Users.AnyAsync(u => u.TenantId == tenantId && u.UserCode == code))
                return code;
        }
        return $"USR{Guid.NewGuid().ToString()[..8].ToUpper()}";
    }
}
