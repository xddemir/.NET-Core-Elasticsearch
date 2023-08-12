using System.Collections.Immutable;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using ElasticSearch.API.Models.ECommerceModel;

namespace ElasticSearch.API.Repository;

public class ECommerceRepository
{
    private readonly ElasticsearchClient _client;

    public ECommerceRepository(ElasticsearchClient client)
    {
        _client = client;
    }

    private const string indexName = "kibana_sample_data_ecommerce";

    public async Task<ImmutableList<ECommerce>> TermQuery(string customerFirstName)
    {
        // var result =
        //     await _client.SearchAsync<ECommerce>(
        //         s => s.Index(indexName).Query(
        //             q => q.Term(
        //                 t => t.Field("customer_first_name.keyword").Value(customerFirstName))));
        //

        // var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
        //     .Query(q => q.Term(t => t.Field(f => f.CustomerFirstName.Suffix("keyword")))));

        var termQuery = new TermQuery("customer_first_name.keyword")
            { Value = customerFirstName, CaseInsensitive = true };

        var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
            .Query(termQuery));
        
        foreach (var hit in result.Hits) hit.Source.Id = hit.Id;
        
        return result.Documents.ToImmutableList();
    }
}