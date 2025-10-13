namespace ProjForum.Forum.Domain.Entities;

public class PostTag
{
    public Guid PostId { get; set; }
    public Post Post { get; set; } = null!;

    public Guid TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}