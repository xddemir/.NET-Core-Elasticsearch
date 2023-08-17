using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using ElasticSearch.Models.ECommerceModel;
using Elasticsearch.WEB.Models;
using Elasticsearch.WEB.ViewModel;

namespace Elasticsearch.WEB.Repositories;

public class ECommerceRepository
{
    private readonly ElasticsearchClient _client;
    private const string indexName = "kibana_sample_data_ecommerce";


    public ECommerceRepository(ElasticsearchClient client)
    {
        _client = client;
    }
    
    
    public async Task<(List<ECommerce> list, long count)> SearchAsync(ECommerceSearchViewModel searchViewModel, int page, int pageSize)
    {
        List<Action<QueryDescriptor<ECommerce>>> listQuery = new();

        if (searchViewModel is null)
        {
            listQuery.Add(q => q.MatchAll());
            return await CalculatePage(page, pageSize, listQuery);
        }

        if (!string.IsNullOrEmpty(searchViewModel.CategoryName))
        {
            // Full text query - CategoryName
            listQuery.Add((q) => q.Match(m => m
                    .Field(f => f.Category)
                    .Query(searchViewModel.CategoryName)));
        }
        
        if (!string.IsNullOrEmpty(searchViewModel.CustomerFullName))
        {
            // Full text query - CustomerName
            listQuery.Add((q) => q.Match(m => m
                .Field(f => f.CustomerFullName)
                .Query(searchViewModel.CustomerFullName)));
        }
        
        if (searchViewModel.OrderDateStart.HasValue)
        {
            // Term Level text query - CustomerName
            listQuery.Add((q) => 
                q.Range(r => 
                    r.DateRange(dr =>
                    dr.Field(f => f.OrderDate).Gte(searchViewModel.OrderDateStart.Value))));
        }
        
        if (searchViewModel.OrderDateEnd.HasValue)
        {
            // Term Level text query - CustomerName
            listQuery.Add((q) => 
                q.Range(r => 
                    r.DateRange(dr =>
                        dr.Field(f => f.OrderDate)
                            .Lte(searchViewModel.OrderDateEnd.Value))));
        }

        if (!string.IsNullOrEmpty(searchViewModel.Gender))
        {
            listQuery.Add(q => q.Term(t => 
                t.Field(tf => tf.Gender).Value(searchViewModel.Gender).CaseInsensitive()));
        }

        if (!listQuery.Any())
        {
            listQuery.Add(q => q.MatchAll());
        }

        return await CalculatePage(page, pageSize, listQuery);


    }

    private async Task<(List<ECommerce> list, long count)> CalculatePage(int page, int pageSize, 
        List<Action<QueryDescriptor<ECommerce>>> listQuery)
    {
        var pageFrom = (page - 1) * pageSize;
        var result = await _client.SearchAsync<ECommerce>(s =>
            s.Index(indexName).Size(pageSize)
                .From(pageFrom)
                .Query(q => q
                    .Bool(b => b
                        .Must(listQuery.ToArray()))));

        foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

        return (list: result.Documents.ToList(), result.Total);
    }
}