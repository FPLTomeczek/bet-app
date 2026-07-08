using System.ComponentModel.DataAnnotations;

namespace BetApp.Api.Dtos;

public record ArticleResponse(int Id, string Title, string Content, string? Author, int? SportCategoryId, DateTime? PublishedAt);

public record CreateArticleRequest(
    [Required][MaxLength(200)] string Title,
    [Required] string Content,
    [MaxLength(120)] string? Author,
    int? SportCategoryId,
    DateTime? PublishedAt);

public record UpdateArticleRequest(
    [Required][MaxLength(200)] string Title,
    [Required] string Content,
    [MaxLength(120)] string? Author,
    int? SportCategoryId,
    DateTime? PublishedAt);
