namespace BetApp.Api.Models;

public class Article
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public string? Author { get; set; }
    public int? SportCategoryId { get; set; }
    public DateTime? PublishedAt { get; set; }

    public SportCategory? SportCategory { get; set; }
}
