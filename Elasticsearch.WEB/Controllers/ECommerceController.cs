using Elasticsearch.WEB.Models;
using Elasticsearch.WEB.Services;
using Microsoft.AspNetCore.Mvc;

namespace Elasticsearch.WEB.Controllers;

public class ECommerceController : Controller
{
    private readonly ECommerceService _service;

    public ECommerceController(ECommerceService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Search([FromQuery] SearchPageViewModel searchPageViewModel)
    {
        var (eCommerceList, totalCount, pageLinkCount ) = await _service.SearchAsync(
                                                searchPageViewModel.SearchViewModel, 
                                                searchPageViewModel.Page,
                                                searchPageViewModel.PageSize);
        
        searchPageViewModel.ViewModels = eCommerceList;
        searchPageViewModel.TotalCount = totalCount;
        searchPageViewModel.PageLinkCount = pageLinkCount;
        
        return View(searchPageViewModel);
    }
}