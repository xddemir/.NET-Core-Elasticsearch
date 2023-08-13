using Elasticsearch.WEB.Services;
using Elasticsearch.WEB.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Elasticsearch.WEB.Controllers;

public class BlogController : Controller
{
    private readonly BlogService _blogService;

    public BlogController(BlogService blogService)
    {
        _blogService = blogService;
    }
    
    public IActionResult Save()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Save(BlogCreateViewModel model)
    {
        var response = await _blogService.SaveAsync(model);

        if (!response){
            TempData["result"] = "failed to save";
            return RedirectToAction(nameof(BlogController.Save));
        }
        
        TempData["result"] = "succeed to save";
        return RedirectToAction(nameof(BlogController.Save));

    }
}