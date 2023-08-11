using System.Collections.Immutable;
using Elastic.Clients.Elasticsearch;
using ElasticSearch.API.DTOs;
using ElasticSearch.API.Models;

namespace ElasticSearch.API.Repository;

public class ProductRepository
{
    private readonly ElasticsearchClient _client;

    public ProductRepository(ElasticsearchClient client)
    {
        _client = client;
    }


    public async Task<Product?> SaveAsync(Product newProduct)
    {
        newProduct.Created = DateTime.Now;

        var response = await _client.IndexAsync(newProduct, 
            x => x.Index("products")
            .Id(Guid.NewGuid().ToString()));
    
        if (!response.IsSuccess()) return null;

        newProduct.Id = response.Id;

        return newProduct;
    }

    public async Task<ImmutableList<Product>> GetAllAsync()
    {
        var result = await _client
                .SearchAsync<Product>(s => s.Index("products")
                .Query(q => q.MatchAll()));

        foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

        return result.Documents.ToImmutableList();
    }

    public async Task<Product> GetProductByIdAsync(string id)
    {
        var response = await _client.GetAsync<Product>(id, x => x.Index("products"));

        if (!response.IsSuccess()) return null;
        
        response.Source.Id = response.Id;
        
        return response.Source;
    }

    public async Task<DeleteResponse> DeleteAsync(string id)
    {
        var response = await _client.DeleteAsync<Product>(
            id, 
            x => 
                x.Index("products"));
        return response;
    }

    public async Task<bool> UpdateAsync(ProductUpdateDto productUpdateDto)
    {
        var response = await _client
            .UpdateAsync<Product, ProductUpdateDto>("" +
                                                    "products", 
                                                    productUpdateDto.Id, 
                                        x => 
                                                    x.Doc(productUpdateDto));
        return response.IsSuccess();
    }
    
}