using System.Collections.Immutable;
using ElasticSearch.API.DTOs;
using ElasticSearch.API.Models;
using Nest;

namespace ElasticSearch.API.Repository;

public class ProductRepository
{
    private readonly ElasticClient _client;

    public ProductRepository(ElasticClient client)
    {
        _client = client;
    }


    public async Task<Product?> SaveAsync(Product newProduct)
    {
        newProduct.Created = DateTime.Now;

        var response = await _client.IndexAsync(newProduct, 
            x => x.Index("products")
            .Id(Guid.NewGuid().ToString()));

        if (!response.IsValid) return null;

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

        if (!response.IsValid) return null;
        
        response.Source.Id = response.Id;
        
        return response.Source;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var response = await _client.DeleteAsync<Product>(id, x => x.Index("products"));
        return response.IsValid;
    }

    public async Task<bool> UpdateAsync(ProductUpdateDto productUpdateDto)
    {
        var response = await _client
            .UpdateAsync<Product, ProductUpdateDto>(productUpdateDto.Id,
            x => x.Index("products").Doc(productUpdateDto));

        return response.IsValid;
    }
    
}