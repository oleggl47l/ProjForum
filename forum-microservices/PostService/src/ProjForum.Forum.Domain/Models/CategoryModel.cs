namespace ProjForum.Forum.Domain.Models;

public class CategoryModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
}