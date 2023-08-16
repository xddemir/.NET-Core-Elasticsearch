using Elasticsearch.WEB.ViewModel;

namespace Elasticsearch.WEB.Models;

public class SearchPageViewModel
{
    public long TotalCount { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public long PageLinkCount { get; set; }
    public List<ECommerceViewModel> ViewModels { get; set; }
    public ECommerceSearchViewModel SearchViewModel { get; set; }

    public string CreatePage(HttpRequest req, int page, int pageSize)
    {
        var currentUrl = new Uri($"{req.Scheme}://{req.Host}{req.Path}{req.QueryString}").AbsoluteUri;
        if (currentUrl.Contains("page", StringComparison.OrdinalIgnoreCase))
        {
            currentUrl = currentUrl.Replace($"Page={Page}", $"Page={page}", StringComparison.OrdinalIgnoreCase);
            currentUrl = currentUrl.Replace($"Page={PageSize}", $"Page={pageSize}", StringComparison.OrdinalIgnoreCase);
        }
        else
        {
            currentUrl = $"{currentUrl}?Page={page}";
            currentUrl = $"{currentUrl}&PageSize={pageSize}";
        }

        return currentUrl;
    }
}