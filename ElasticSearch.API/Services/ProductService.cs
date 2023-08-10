using System.Collections.Immutable;
using System.Net;
using ElasticSearch.API.DTOs;
using ElasticSearch.API.Models;
using ElasticSearch.API.Repository;

namespace ElasticSearch.API.Services;

public class ProductService
{
    private readonly ProductRepository _productRepository;

    public ProductService(ProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ResponseDto<ProductDto>> SaveAsync(ProductCreateDto request)
    {
        var response = await _productRepository.SaveAsync(request.CreateProduct());
        
        if (response == null)
            return ResponseDto<ProductDto>.Fail(new List<string> { "an error occured" },
                HttpStatusCode.ServiceUnavailable);

        return ResponseDto<ProductDto>.Success(response.CreateDto(), HttpStatusCode.Created);
    }
    
    public async Task<ResponseDto<List<ProductDto>>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();
        var productListDto = new List<ProductDto>();

        foreach (var x in products)
        {
            if (x.Feature is null)
                productListDto.Add(new ProductDto(x.Id, x.Name, x.Price, x.Stock, null));
            else
                productListDto.Add(new ProductDto(x.Id, x.Name, x.Price, x.Stock, new ProductFeatureDto(
                    x.Feature.Width, x.Feature.Height, x.Feature.Color.ToString())));
        }
        
        return ResponseDto<List<ProductDto>>.Success(productListDto, HttpStatusCode.OK);
    }

    public async Task<ResponseDto<ProductDto>> GetProductByIdAsync(string id)
    {
        var product = await _productRepository.GetProductByIdAsync(id);

        if (product == null) return ResponseDto<ProductDto>.Fail("item not found", HttpStatusCode.NotFound);

        return ResponseDto<ProductDto>.Success(product.CreateDto(), HttpStatusCode.OK);
    }

    public async Task<ResponseDto<bool>> UpdateAsync(ProductUpdateDto productUpdateDto)
    {
        var responseProducts = await _productRepository.UpdateAsync(productUpdateDto);

        if (!responseProducts)
            return ResponseDto<bool>.Fail("error occurred during update", HttpStatusCode.InternalServerError);

        return ResponseDto<bool>.Success(true, HttpStatusCode.NoContent);
    }

    public async Task<ResponseDto<bool>> DeleteAsync(string id)
    {
        var isDeleted = await _productRepository.DeleteAsync(id);
        
        if (!isDeleted)
            return ResponseDto<bool>.Fail("service is unable to delete", HttpStatusCode.InternalServerError);
        
        return ResponseDto<bool>.Success(true, HttpStatusCode.NoContent);
    }

}