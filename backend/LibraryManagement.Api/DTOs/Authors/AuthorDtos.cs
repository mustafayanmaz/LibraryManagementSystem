using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Api.DTOs.Authors;

public sealed class AuthorUpsertRequest
{
    [Required]
    [StringLength(120, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Biography { get; set; }
}

public sealed class AuthorResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Biography { get; set; }

    public int BookCount { get; set; }
}