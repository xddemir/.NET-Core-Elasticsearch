using System.ComponentModel.DataAnnotations;

namespace Elasticsearch.WEB.ViewModel;

public class BlogCreateViewModel
{
    [Required]
    [Display(Name = "Blog Title")]
    public string Title { get; set; } = null!;

    [Required]
    [Display(Name = "Blog Content")]
    public string Content { get; set; } = null!;

    [Display(Name = "Blog Tags")] public string Tags { get; set; } = null!;
}