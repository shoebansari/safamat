namespace Matrimonial.AdminApi.DTOs.SubscriptionPlans;

public class SubscriptionPlanDto
{
    public Guid PlanId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int DurationDays { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
}

public class CreateSubscriptionPlanRequest
{
    public string PlanName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int DurationDays { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateSubscriptionPlanRequest
{
    public string? PlanName { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int? DurationDays { get; set; }
    public bool? IsActive { get; set; }
}
