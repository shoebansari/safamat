using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Matrimonial.AdminApi.Configurations;
using Matrimonial.AdminApi.Data;
using Matrimonial.AdminApi.DTOs.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Matrimonial.AdminApi.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
}

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly JwtSettings _jwtSettings;

    public AuthService(ApplicationDbContext context, IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var admin = await _context.AdminUsers
            .FirstOrDefaultAsync(a => a.AdminUserName == request.AdminUserName && a.IsActive);

        if (admin == null || !BCrypt.Net.BCrypt.Verify(request.Password, admin.Password))
            return null;

        admin.LastLogin = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);
        var token = GenerateToken(admin, expiresAt);

        return new LoginResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            Admin = new AdminUserInfo
            {
                AdminId = admin.AdminId,
                AdminUserName = admin.AdminUserName,
                FirstName = admin.FirstName,
                LastName = admin.LastName,
                Email = admin.Email
            }
        };
    }

    private string GenerateToken(Entities.AdminUser admin, DateTime expiresAt)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, admin.AdminId.ToString()),
            new Claim(ClaimTypes.Name, admin.AdminUserName),
            new Claim(ClaimTypes.Email, admin.Email),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
