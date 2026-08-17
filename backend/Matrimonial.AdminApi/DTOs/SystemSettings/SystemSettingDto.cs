namespace Matrimonial.AdminApi.DTOs.SystemSettings;

public class SystemSettingDto
{
    public Guid SettingId { get; set; }
    public string SettingKey { get; set; } = string.Empty;
    public string SettingValue { get; set; } = string.Empty;
}

public class CreateSystemSettingRequest
{
    public string SettingKey { get; set; } = string.Empty;
    public string SettingValue { get; set; } = string.Empty;
}

public class UpdateSystemSettingRequest
{
    public string SettingValue { get; set; } = string.Empty;
}
