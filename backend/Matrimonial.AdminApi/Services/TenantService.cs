using Matrimonial.AdminApi.Common;
using Matrimonial.AdminApi.Data;
using Matrimonial.AdminApi.DTOs.Tenants;
using Matrimonial.AdminApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matrimonial.AdminApi.Services;

public interface ITenantService
{
    Task<PagedResult<TenantDto>> GetAllAsync(int page, int pageSize, string? search, bool? isActive);
    Task<TenantDto?> GetByIdAsync(Guid id);
    Task<TenantDto> CreateAsync(CreateTenantRequest request, Guid adminId);
    Task<TenantDto?> UpdateAsync(Guid id, UpdateTenantRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<(bool TenantCodeExists, bool CompanyNameExists, bool UserNameExists)> ExistsAsync(
        string? tenantCode, string? companyName, string? userName, Guid? excludeTenantId = null);
}

public class TenantService : ITenantService
{
    private readonly ApplicationDbContext _context;

    public TenantService(ApplicationDbContext context) => _context = context;

    public async Task<PagedResult<TenantDto>> GetAllAsync(int page, int pageSize, string? search, bool? isActive)
    {
        var query = _context.Tenants.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(t =>
                t.TenantCode.Contains(search) ||
                t.CompanyName.Contains(search) ||
                t.OwnerName.Contains(search) ||
                t.UserName.Contains(search) ||
                t.Email.Contains(search));
        }

        if (isActive.HasValue)
            query = query.Where(t => t.IsActive == isActive.Value);

        var total = await query.CountAsync();
        var tenants = await query
            .OrderByDescending(t => t.CreatedOn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        var items = tenants.Select(MapToDto).ToList();

        return new PagedResult<TenantDto> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
    }

    public async Task<TenantDto?> GetByIdAsync(Guid id)
    {
        var tenant = await _context.Tenants.FindAsync(id);
        return tenant == null ? null : MapToDto(tenant);
    }

    public async Task<TenantDto> CreateAsync(CreateTenantRequest request, Guid adminId)
    {
        if (await _context.Tenants.AnyAsync(t => t.TenantCode.ToLower() == request.TenantCode.ToLower()))
            throw new InvalidOperationException("Tenant code already exists.");

        if (await _context.Tenants.AnyAsync(t => t.CompanyName.ToLower() == request.CompanyName.ToLower()))
            throw new InvalidOperationException("Company name already exists.");

        if (await _context.Tenants.AnyAsync(t => t.UserName.ToLower() == request.UserName.ToLower()))
            throw new InvalidOperationException("Username already exists.");

        var tenant = new Tenant
        {
            TenantId = Guid.NewGuid(),
            TenantCode = request.TenantCode,
            CompanyName = request.CompanyName,
            OwnerName = request.OwnerName,
            UserName = request.UserName,
            Password = request.Password,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            City = request.City,
            State = request.State,
            Country = request.Country,
            ZipCode = request.ZipCode,
            LogoUrl = request.LogoUrl,
            DatabaseName = request.DatabaseName,
            DatabaseServer = request.DatabaseServer,
            ConnectionString = request.ConnectionString,
            IsActive = request.IsActive,
            CreatedBy = adminId,
            CreatedOn = DateTime.UtcNow
        };

        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync();
        return MapToDto(tenant);
    }

    public async Task<TenantDto?> UpdateAsync(Guid id, UpdateTenantRequest request)
    {
        var tenant = await _context.Tenants.FindAsync(id);
        if (tenant == null) return null;

        if (request.CompanyName != null) tenant.CompanyName = request.CompanyName;
        if (request.OwnerName != null) tenant.OwnerName = request.OwnerName;
        if (!string.IsNullOrWhiteSpace(request.UserName))
        {
            if (await _context.Tenants.AnyAsync(t => t.TenantId != id && t.UserName.ToLower() == request.UserName.ToLower()))
                throw new InvalidOperationException("Username already exists.");
            tenant.UserName = request.UserName.Trim();
        }
        else if (string.IsNullOrWhiteSpace(tenant.UserName))
            throw new InvalidOperationException("Username is required.");

        if (!string.IsNullOrWhiteSpace(request.Password))
            tenant.Password = request.Password;
        else if (string.IsNullOrWhiteSpace(tenant.Password))
            throw new InvalidOperationException("Password is required.");
        if (request.Email != null) tenant.Email = request.Email;
        if (request.Phone != null) tenant.Phone = request.Phone;
        if (request.Address != null) tenant.Address = request.Address;
        if (request.City != null) tenant.City = request.City;
        if (request.State != null) tenant.State = request.State;
        if (request.Country != null) tenant.Country = request.Country;
        if (request.ZipCode != null) tenant.ZipCode = request.ZipCode;
        if (request.LogoUrl != null) tenant.LogoUrl = request.LogoUrl;
        if (request.DatabaseName != null) tenant.DatabaseName = request.DatabaseName;
        if (request.DatabaseServer != null) tenant.DatabaseServer = request.DatabaseServer;
        if (request.ConnectionString != null) tenant.ConnectionString = request.ConnectionString;
        if (request.IsActive.HasValue) tenant.IsActive = request.IsActive.Value;

        tenant.UpdatedOn = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return MapToDto(tenant);
    }

    public async Task<(bool TenantCodeExists, bool CompanyNameExists, bool UserNameExists)> ExistsAsync(
        string? tenantCode, string? companyName, string? userName, Guid? excludeTenantId = null)
    {
        var query = _context.Tenants.AsQueryable();
        if (excludeTenantId.HasValue)
            query = query.Where(t => t.TenantId != excludeTenantId.Value);

        var codeExists = !string.IsNullOrWhiteSpace(tenantCode) &&
            await query.AnyAsync(t => t.TenantCode.ToLower() == tenantCode.ToLower());
        var nameExists = !string.IsNullOrWhiteSpace(companyName) &&
            await query.AnyAsync(t => t.CompanyName.ToLower() == companyName.ToLower());
        var userNameExists = !string.IsNullOrWhiteSpace(userName) &&
            await query.AnyAsync(t => t.UserName.ToLower() == userName.ToLower());
        return (codeExists, nameExists, userNameExists);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var tenant = await _context.Tenants.FindAsync(id);
        if (tenant == null) return false;

        tenant.IsActive = false;
        tenant.UpdatedOn = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    private static TenantDto MapToDto(Tenant t) => new()
    {
        TenantId = t.TenantId,
        TenantCode = t.TenantCode,
        CompanyName = t.CompanyName,
        OwnerName = t.OwnerName,
        UserName = t.UserName,
        Password = t.Password,
        Email = t.Email,
        Phone = t.Phone,
        Address = t.Address,
        City = t.City,
        State = t.State,
        Country = t.Country,
        ZipCode = t.ZipCode,
        LogoUrl = t.LogoUrl,
        DatabaseName = t.DatabaseName,
        DatabaseServer = t.DatabaseServer,
        IsActive = t.IsActive,
        CreatedBy = t.CreatedBy,
        CreatedOn = t.CreatedOn,
        UpdatedOn = t.UpdatedOn
    };
}
