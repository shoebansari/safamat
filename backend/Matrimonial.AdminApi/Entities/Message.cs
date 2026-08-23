namespace Matrimonial.AdminApi.Entities;

public class Message
{
    public Guid MessageId { get; set; }
    public Guid SenderUserId { get; set; }
    public Guid ReceiverUserId { get; set; }
    public string MessageText { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime SentOn { get; set; } = DateTime.UtcNow;

    public User Sender { get; set; } = null!;
    public User Receiver { get; set; } = null!;
}
