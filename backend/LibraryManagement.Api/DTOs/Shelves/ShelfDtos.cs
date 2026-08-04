using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Api.DTOs.Shelves;

public sealed class ShelfUpsertRequest
{
    [Required]
    [StringLength(20, MinimumLength = 2)]
    public string Code { get; set; } = string.Empty;

    [Range(0, 20)]
    public int Floor { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Section { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}

public sealed class ShelfResponse
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public int Floor { get; set; }

    public string Section { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Location { get; set; } = string.Empty;

    public int BookCount { get; set; }
}