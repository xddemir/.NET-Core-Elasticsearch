using Elasticsearch.WEB.Models;
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

    public IActionResult Search()
    {
        return View(new List<Blog>());
    }
    
    [HttpPost]
    public async Task<IActionResult> Search(string searchText)
    {
        var blogList = await _blogService.SearchAsync(searchText);
        return View(blogList);
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