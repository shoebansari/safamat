namespace Matrimonial.AdminApi.Entities;

public class InterestRequest
{
    public Guid InterestId { get; set; }
    public Guid SenderUserId { get; set; }
    public Guid ReceiverUserId { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime SentOn { get; set; } = DateTime.UtcNow;
    public DateTime? RespondedOn { get; set; }

    public User Sender { get; set; } = null!;
    public User Receiver { get; set; } = null!;
}
