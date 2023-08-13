using Elasticsearch.WEB.Models;
using Elasticsearch.WEB.Repositories;
using Elasticsearch.WEB.ViewModel;

namespace Elasticsearch.WEB.Services;

public class BlogService
{
    // Dependency Inversion and Inversion of Control = dependency injection
    private readonly BlogRepository _repository;

    public BlogService(BlogRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> SaveAsync(BlogCreateViewModel model)
    {
        Blog newBlog = new Blog
        {
            Title = model.Title,
            Content = model.Content,
            UserId = Guid.NewGuid(),
            Tags = model.Tags.Split(",")
        };

        var isCreated = await _repository.SaveAsync(newBlog);
        return isCreated != null;
    }

    public async Task<List<Blog>> SearchAsync(string searchText)
    {
        return await _repository.SearchAsync(searchText);
    }
}