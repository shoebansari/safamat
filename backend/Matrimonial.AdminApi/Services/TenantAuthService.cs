using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Matrimonial.AdminApi.Configurations;
using Matrimonial.AdminApi.Data;
using Matrimonial.AdminApi.DTOs.TenantPanel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Matrimonial.AdminApi.Services;

public interface ITenantAuthService
{
    Task<(TenantLoginResponse? Result, string? Error)> LoginAsync(TenantLoginRequest request);
}

public class TenantAuthService : ITenantAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly JwtSettings _jwtSettings;

    public TenantAuthService(ApplicationDbContext context, IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<(TenantLoginResponse? Result, string? Error)> LoginAsync(TenantLoginRequest request)
    {
        var tenants = await _context.Tenants
            .Where(t => t.UserName.ToLower() == request.UserName.ToLower())
            .ToListAsync();

        if (tenants.Count == 0)
            return (null, "Invalid username or password.");

        var match = tenants.FirstOrDefault(t => t.Password == request.Password);
        if (match == null)
            return (null, "Invalid username or password.");

        if (!match.IsActive)
            return (null, "Your tenant account is inactive. Please contact the administrator.");

        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);
        var token = GenerateToken(match, expiresAt);

        return (new TenantLoginResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            Tenant = new TenantSessionInfo
            {
                TenantId = match.TenantId,
                TenantCode = match.TenantCode,
                CompanyName = match.CompanyName,
                UserName = match.UserName,
                Email = match.Email
            }
        }, null);
    }

    private string GenerateToken(Entities.Tenant tenant, DateTime expiresAt)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, tenant.TenantId.ToString()),
            new Claim("tenant_id", tenant.TenantId.ToString()),
            new Claim(ClaimTypes.Name, tenant.UserName),
            new Claim(ClaimTypes.Email, tenant.Email),
            new Claim(ClaimTypes.Role, "Tenant")
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
