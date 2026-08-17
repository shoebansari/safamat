using Matrimonial.AdminApi.Common;
using Matrimonial.AdminApi.Data;
using Matrimonial.AdminApi.DTOs.SystemSettings;
using Matrimonial.AdminApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matrimonial.AdminApi.Services;

public interface ISystemSettingService
{
    Task<PagedResult<SystemSettingDto>> GetAllAsync(int page, int pageSize, string? search);
    Task<SystemSettingDto?> GetByIdAsync(Guid id);
    Task<SystemSettingDto?> GetByKeyAsync(string key);
    Task<SystemSettingDto> CreateAsync(CreateSystemSettingRequest request);
    Task<SystemSettingDto?> UpdateAsync(Guid id, UpdateSystemSettingRequest request);
    Task<bool> DeleteAsync(Guid id);
}

public class SystemSettingService : ISystemSettingService
{
    private readonly ApplicationDbContext _context;

    public SystemSettingService(ApplicationDbContext context) => _context = context;

    public async Task<PagedResult<SystemSettingDto>> GetAllAsync(int page, int pageSize, string? search)
    {
        var query = _context.SystemSettings.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(s => s.SettingKey.Contains(search));

        var total = await query.CountAsync();
        var settings = await query
            .OrderBy(s => s.SettingKey)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        var items = settings.Select(MapToDto).ToList();

        return new PagedResult<SystemSettingDto> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
    }

    public async Task<SystemSettingDto?> GetByIdAsync(Guid id)
    {
        var setting = await _context.SystemSettings.FindAsync(id);
        return setting == null ? null : MapToDto(setting);
    }

    public async Task<SystemSettingDto?> GetByKeyAsync(string key)
    {
        var setting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.SettingKey == key);
        return setting == null ? null : MapToDto(setting);
    }

    public async Task<SystemSettingDto> CreateAsync(CreateSystemSettingRequest request)
    {
        if (await _context.SystemSettings.AnyAsync(s => s.SettingKey == request.SettingKey))
            throw new InvalidOperationException("Setting key already exists.");

        var setting = new SystemSetting
        {
            SettingId = Guid.NewGuid(),
            SettingKey = request.SettingKey,
            SettingValue = request.SettingValue
        };

        _context.SystemSettings.Add(setting);
        await _context.SaveChangesAsync();
        return MapToDto(setting);
    }

    public async Task<SystemSettingDto?> UpdateAsync(Guid id, UpdateSystemSettingRequest request)
    {
        var setting = await _context.SystemSettings.FindAsync(id);
        if (setting == null) return null;

        setting.SettingValue = request.SettingValue;
        await _context.SaveChangesAsync();
        return MapToDto(setting);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var setting = await _context.SystemSettings.FindAsync(id);
        if (setting == null) return false;

        _context.SystemSettings.Remove(setting);
        await _context.SaveChangesAsync();
        return true;
    }

    private static SystemSettingDto MapToDto(SystemSetting s) => new()
    {
        SettingId = s.SettingId,
        SettingKey = s.SettingKey,
        SettingValue = s.SettingValue
    };
}
