using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Api.DTOs.Categories;

public sealed class CategoryUpsertRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}

public sealed class CategoryResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int BookCount { get; set; }
}