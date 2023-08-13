using ElasticSearch.API.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearch.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class ECommerceController : ControllerBase
{
    private readonly ECommerceRepository _repository;

    public ECommerceController(ECommerceRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> TermQuery(string customerFirstName)
    {
        return Ok(await _repository.TermQuery(customerFirstName));
    }
    
    [HttpPost]
    public async Task<IActionResult> TermsQuery(List<string> customerFirstNames)
    {
        return Ok(await _repository.TermsQuery(customerFirstNames));
    }
    
    [HttpGet]
    public async Task<IActionResult> PrefixQuery(string customerFirstName)
    {
        return Ok(await _repository.PrefixQuery(customerFirstName));
    }
    
    [HttpGet]
    public async Task<IActionResult> RangeQuery(double fromPrice, double toPrice)
    {
        return Ok(await _repository.RangeQuery(fromPrice, toPrice));
    }
    
    [HttpGet]
    public async Task<IActionResult> MatchAll()
    {
        return Ok(await _repository.MatchAllQuery());
    }
    
    [HttpGet]
    public async Task<IActionResult> PaginationQuery(int page, int pageSize)
    {
        return Ok(await _repository.PaginationQuery(page, pageSize));
    }
    
    [HttpGet]
    public async Task<IActionResult> WildCardQuery(string customerFullName)
    {
        return Ok(await _repository.WIldCardQuery(customerFullName));
    }
    
    [HttpGet]
    public async Task<IActionResult> FuzzyQuery(string customerFullName)
    {
        return Ok(await _repository.FuzzyQuery(customerFullName));
    }
    
    [HttpGet]
    public async Task<IActionResult> MatchQueryFullText(string categoryName)
    {
        return Ok(await _repository.MatchQueryFullText(categoryName));
    }

    [HttpGet]
    public async Task<IActionResult> MatchQueryBoolPrefix(string categoryName)
    {
        return Ok(await _repository.MatchQueryBoolPrefix(categoryName));

    }
    
    [HttpGet]
    public async Task<IActionResult> MatchPhraseQuery(string categoryName)
    {
        return Ok(await _repository.MatchPhraseFullText(categoryName));
    }

    [HttpGet]
    public async Task<IActionResult> CompoundQueryBooleanAsync(string cityName,
        double taxFulTotalPrice, string categoryName, string manufacturer) {
        return Ok(await _repository.CompoundQueryBooleanAsync(cityName, taxFulTotalPrice, categoryName, manufacturer));
    }
    
    [HttpGet]
    public async Task<IActionResult> MultiMatchQuery(string name)
    {
        return Ok(await _repository.MultiMatchQuery(name));
    }

}