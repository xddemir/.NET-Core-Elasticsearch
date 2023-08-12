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
}