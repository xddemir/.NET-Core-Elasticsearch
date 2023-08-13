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
        var termQuery = new TermQuery("customer_first_name.keyword")
            { Value = customerFirstName, CaseInsensitive = true };

        var result = await _client.SearchAsync<ECommerce>(
            s => s.Index(indexName)
                .Query(termQuery));

        foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

        return result.Documents.ToImmutableList();
    }

    public async Task<ImmutableList<ECommerce>> TermsQuery(List<string> customerFirstNameList)
    {
        List<FieldValue> terms = new List<FieldValue>();

        customerFirstNameList.ForEach(x => { terms.Add(x); });

        var termsQuery = new TermsQuery()
        {
            Field = "customer_first_name.keyword",
            Terms = new TermsQueryField(terms.AsReadOnly())
        };

        var result = await _client.SearchAsync<ECommerce>(
            s => s.Index(indexName).Query(termsQuery));

        return result.Documents.ToImmutableList();
    }

    public async Task<ImmutableList<ECommerce>> PrefixQuery(string customerFullName)
    {
        var result = await _client.SearchAsync<ECommerce>(
            s => s.Index(indexName)
                .Query(q =>
                    q.Prefix(p =>
                        p.Field(f => f.CustomerFullName.Suffix("keyword")).Value(customerFullName))));

        return result.Documents.ToImmutableList();
    }

    public async Task<ImmutableList<ECommerce>> RangeQuery(double fromPrice, double toPrice)
    {
        var result = await _client.SearchAsync<ECommerce>(
            s =>
                s.Index(indexName)
                    .Query(q =>
                        q.Range(r =>
                            r.NumberRange(nr =>
                                nr.Field(f => f.TaxFulTotalPrice).Gte(fromPrice).Lte(toPrice)))));

        return result.Documents.ToImmutableList();
    }

    public async Task<ImmutableList<ECommerce>> MatchAllQuery()
    {
        var result = await _client.SearchAsync<ECommerce>(s => s
            .Index(indexName)
            .Size(50)
            .Query(q => q.MatchAll()));

        return result.Documents.ToImmutableList();
    }

    public async Task<ImmutableList<ECommerce>> PaginationQuery(int page, int pageSize)
    {
        var pagefrom = (page - 1) * pageSize;

        var result = await _client.SearchAsync<ECommerce>(s => s
            .Index(indexName)
            .Size(pageSize)
            .From(pagefrom)
            .Query(q => q
                .MatchAll()));

        return result.Documents.ToImmutableList();
    }

    public async Task<ImmutableList<ECommerce>> WIldCardQuery(string customerFullName)
    {
        var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
            .Query(q => q
                .Wildcard(w => w
                    .Field(f => f.CustomerFullName.Suffix("keyword"))
                    .Wildcard(customerFullName))));

        foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

        return result.Documents.ToImmutableList();
    }

    public async Task<ImmutableList<ECommerce>> FuzzyQuery(string customerName)
    {
        var result = await _client.SearchAsync<ECommerce>(s =>
            s.Index(indexName)
                .Query(q => q
                    .Fuzzy(fu => fu
                        .Field(f => f.CustomerFirstName.Suffix("keyword"))
                        .Value(customerName)
                        .Fuzziness(new Fuzziness(1)))).Sort(sort => sort
                    .Field(f => f.TaxFulTotalPrice, new FieldSort(){Order = SortOrder.Asc})));
        
        foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

        return result.Documents.ToImmutableList();
    }

    public async Task<ImmutableList<ECommerce>> MatchQueryFullText(string categoryName)
    {
        var result = await _client.SearchAsync<ECommerce>(s => s
            .Index(indexName).Size(1000).Query(q => q
                .Match(m => m.Field(f => f.Category).Query(categoryName))));

        foreach (var hit in result.Hits) hit.Source.Id = hit.Id;
        
        return result.Documents.ToImmutableList();
    }
    
    public async Task<ImmutableList<ECommerce>> MatchQueryBoolPrefix(string categoryName)
    {
        var result = await _client.SearchAsync<ECommerce>(s => s
            .Index(indexName).Size(1000).Query(q => q
                .MatchBoolPrefix(m => m.Field(f => f.Category).Query(categoryName))));

        foreach (var hit in result.Hits) hit.Source.Id = hit.Id;
        
        return result.Documents.ToImmutableList();
    }
    
    public async Task<ImmutableList<ECommerce>> MatchPhraseFullText(string categoryName)
    {
        var result = await _client.SearchAsync<ECommerce>(s => s
            .Index(indexName).Size(1000).Query(q => q
                .MatchPhrase(m => m.Field(f => f.Category).Query(categoryName))));

        foreach (var hit in result.Hits) hit.Source.Id = hit.Id;
        
        return result.Documents.ToImmutableList();
    }
    
    public async Task<ImmutableList<ECommerce>> CompoundQueryBooleanAsync(string cityName, 
        double taxFulTotalPrice, string categoryName, string manufacturer)
    {
        var result = await _client.SearchAsync<ECommerce>(s => s
            .Index(indexName).Size(1000).Query(q => q.Bool(b => b
                    .Must(m =>
                        m.Term(t =>
                            t.Field("geoip.city_name").Value(cityName)))
                    .MustNot(mn =>
                        mn.Range(r =>
                            r.NumberRange(nr =>
                                nr.Field(f => f
                                    .TaxFulTotalPrice)
                                    .Lte(taxFulTotalPrice))))
                    .Should(s => 
                        s.Term(t => 
                            t.Field(f => 
                                f.Category.Suffix("keyword")).
                                Value(categoryName)))
                    .Filter(f => 
                        f.Term(t => 
                            t.Field("manufacturer.keyword")
                                .Value(manufacturer)))
                )));

        foreach (var hit in result.Hits) hit.Source.Id = hit.Id;
        
        return result.Documents.ToImmutableList();
    }
    
    public async Task<ImmutableList<ECommerce>> CompoundQueryBooleanTwoAsync(string customerFullName)
    {
        var result = await _client.SearchAsync<ECommerce>(s => s
            .Index(indexName).Size(1000).Query(q => q
                .Bool(b => b
                .Should(s => s
                    .Match(m => m
                        .Field(f => f.CustomerFullName)
                        .Query(customerFullName))
                    .Prefix(p => p
                        .Field(f => f.CustomerFullName.Suffix("keyword"))
                        .Value(customerFullName))
                ))
            ));

        foreach (var hit in result.Hits) hit.Source.Id = hit.Id;
        
        return result.Documents.ToImmutableList();
    }
    
    public async Task<ImmutableList<ECommerce>> MultiMatchQuery(string name)
    {
        var result = await _client.SearchAsync<ECommerce>(s => s
            .Index(indexName).Size(1000).Query(q => q
                .MultiMatch(mm => mm
                    .Fields(new Field("customer_first_name")
                    .And(new Field("customer_last_name"))
                    .And(new Field("customer_full_name")))
                    .Query(name))
            ));

        foreach (var hit in result.Hits) hit.Source.Id = hit.Id;
        return result.Documents.ToImmutableList();
    }
    
    
}