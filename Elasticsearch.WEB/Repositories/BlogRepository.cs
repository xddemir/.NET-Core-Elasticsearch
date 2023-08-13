using Elastic.Clients.Elasticsearch;
using Elasticsearch.WEB.Models;

namespace Elasticsearch.WEB.Repositories;

public class BlogRepository
{
    private readonly ElasticsearchClient _client;
    private const string IndexName = "blog";

    public BlogRepository(ElasticsearchClient client)
    {
        _client = client;
    }

    public async Task<Blog?> SaveAsync(Blog newBlog)
    {
        newBlog.Created = DateTime.Now;

        var response = await _client.IndexAsync(newBlog, 
            x => x.Index(IndexName)
                .Id(Guid.NewGuid().ToString()));
    
        if (!response.IsValidResponse) return null;

        newBlog.Id = response.Id;

        return newBlog;    
    }

    public async Task<List<Blog>> SearchAsync(string searchText)
    {
        // title -> full text query - prefix phrase match
        // content -> full text query
        // comma among those queries describes 'or'

        var result = await _client.SearchAsync<Blog>(s => s
            .Index(IndexName).Size(1000).Query(q => q
                .Bool(b => b
                    .Should(
                        s => s.Match(m => m
                            .Field(f => f.Content)
                            .Query(searchText)),
                        s => s.MatchBoolPrefix(p => p
                            .Field(f => f.Title)
                            .Query(searchText)))
                )));

        foreach (var hit in result.Hits) hit.Source.Id = hit.Id;
        return result.Documents.ToList();
    }
}