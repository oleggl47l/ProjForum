using ProjForum.Forum.Domain.Entities;
using ProjForum.Forum.Domain.Interfaces;

namespace ProjForum.Forum.Infrastructure.Data;

public class DatabaseSeeder(ITagRepository tagRepository, ICategoryRepository categoryRepository)
{
    public async Task SeedAsync()
    {
        await SeedTagsAsync();
        await SeedCategoriesAsync();
    }

    private async Task SeedTagsAsync()
    {
        var tagsToSeed = new List<Tag>
        {
            new() { Name = "Test tag 1" },
            new() { Name = "Test tag 2" },
            new() { Name = "Test tag 3" },
            new() { Name = "Test tag 4" },
            new() { Name = "Test tag 5" }
        };

        foreach (var tag in tagsToSeed)
        {
            var tagExists = await tagRepository.TagExistsByNameAsync(tag.Name);
            if (!tagExists)
            {
                await tagRepository.AddAsync(tag);
            }
        }
    }

    private async Task SeedCategoriesAsync()
    {
        var categoriesToSeed = new List<Category>
        {
            new() { Name = "Test category 1", Description = "Category for testing api 1" },
            new() { Name = "Test category 2", Description = "Category for testing api 2" },
            new() { Name = "Test category 3", Description = "Category for testing api 3" },
            new() { Name = "Test category 4", Description = "Category for testing api 4" },
            new() { Name = "Test category 5", Description = "Category for testing api 5" }
        };

        foreach (var category in categoriesToSeed)
        {
            var categoryExists = await categoryRepository.CategoryExistsByNameAsync(category.Name);
            if (!categoryExists)
            {
                await categoryRepository.AddAsync(category);
            }
        }
    }
}