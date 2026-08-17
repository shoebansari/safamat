using Matrimonial.AdminApi.Common;
using Matrimonial.AdminApi.Data;
using Matrimonial.AdminApi.DTOs.AdminUsers;
using Matrimonial.AdminApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matrimonial.AdminApi.Services;

public interface IAdminUserService
{
    Task<PagedResult<AdminUserDto>> GetAllAsync(int page, int pageSize, string? search);
    Task<AdminUserDto?> GetByIdAsync(Guid id);
    Task<AdminUserDto> CreateAsync(CreateAdminUserRequest request);
    Task<AdminUserDto?> UpdateAsync(Guid id, UpdateAdminUserRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> UsernameExistsAsync(string username);
}

public class AdminUserService : IAdminUserService
{
    private readonly ApplicationDbContext _context;

    public AdminUserService(ApplicationDbContext context) => _context = context;

    public async Task<PagedResult<AdminUserDto>> GetAllAsync(int page, int pageSize, string? search)
    {
        var query = _context.AdminUsers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(a =>
                a.AdminUserName.Contains(search) ||
                a.FirstName.Contains(search) ||
                a.LastName.Contains(search) ||
                a.Email.Contains(search));
        }

        var total = await query.CountAsync();
        var admins = await query
            .OrderByDescending(a => a.CreatedOn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        var items = admins.Select(MapToDto).ToList();

        return new PagedResult<AdminUserDto> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
    }

    public async Task<AdminUserDto?> GetByIdAsync(Guid id)
    {
        var admin = await _context.AdminUsers.FindAsync(id);
        return admin == null ? null : MapToDto(admin);
    }

    public async Task<AdminUserDto> CreateAsync(CreateAdminUserRequest request)
    {
        if (await _context.AdminUsers.AnyAsync(a => a.AdminUserName.ToLower() == request.AdminUserName.ToLower()))
            throw new InvalidOperationException("Username already exists.");

        if (await _context.AdminUsers.AnyAsync(a => a.Email == request.Email))
            throw new InvalidOperationException("Email already exists.");

        var admin = new AdminUser
        {
            AdminId = Guid.NewGuid(),
            AdminUserName = request.AdminUserName,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            IsActive = request.IsActive,
            CreatedOn = DateTime.UtcNow
        };

        _context.AdminUsers.Add(admin);
        await _context.SaveChangesAsync();
        return MapToDto(admin);
    }

    public async Task<AdminUserDto?> UpdateAsync(Guid id, UpdateAdminUserRequest request)
    {
        var admin = await _context.AdminUsers.FindAsync(id);
        if (admin == null) return null;

        if (request.FirstName != null) admin.FirstName = request.FirstName;
        if (request.LastName != null) admin.LastName = request.LastName;
        if (request.Email != null) admin.Email = request.Email;
        if (request.Phone != null) admin.Phone = request.Phone;
        if (request.IsActive.HasValue) admin.IsActive = request.IsActive.Value;
        if (!string.IsNullOrEmpty(request.Password))
            admin.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);

        admin.UpdatedOn = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return MapToDto(admin);
    }

    public async Task<bool> UsernameExistsAsync(string username) =>
        await _context.AdminUsers.AnyAsync(a => a.AdminUserName.ToLower() == username.ToLower());

    public async Task<bool> DeleteAsync(Guid id)
    {
        var admin = await _context.AdminUsers.FindAsync(id);
        if (admin == null) return false;

        admin.IsActive = false;
        admin.UpdatedOn = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    private static AdminUserDto MapToDto(AdminUser a) => new()
    {
        AdminId = a.AdminId,
        AdminUserName = a.AdminUserName,
        FirstName = a.FirstName,
        LastName = a.LastName,
        Email = a.Email,
        Phone = a.Phone,
        IsActive = a.IsActive,
        LastLogin = a.LastLogin,
        CreatedOn = a.CreatedOn,
        UpdatedOn = a.UpdatedOn
    };
}
