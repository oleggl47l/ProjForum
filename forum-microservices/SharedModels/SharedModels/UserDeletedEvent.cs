namespace SharedModels;

public class UserDeletedEvent
{
    public string UserId { get; set; }
    public DateTime DeletedAt { get; set; }
}