namespace ProjForum.Forum.Domain.Entities;

public class Post
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public Guid CategoryId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsPublished { get; set; } = true;
    
    public Category? Category { get; set; }
    public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();

}