namespace SharedModels;

public class UserStatusChangedEvent
{
    public string UserId { get; set; }
    public bool Active { get; set; }
    public DateTime ChangedAt { get; set; }
}