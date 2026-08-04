using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Api.DTOs.Books;

public class CreateBookRequest
{
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(13, MinimumLength = 10)]
    public string ISBN { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Range(1000, 2100)]
    public int PublicationYear { get; set; }

    [MaxLength(150)]
    public string? Publisher { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [Range(0, int.MaxValue)]
    public int TotalStock { get; set; }

    [Range(0, int.MaxValue)]
    public int AvailableStock { get; set; }

    [Range(1, int.MaxValue)]
    public int AuthorId { get; set; }

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }

    [Range(1, int.MaxValue)]
    public int ShelfId { get; set; }
}

public sealed class UpdateBookRequest : CreateBookRequest
{
}

public sealed class BookListItemResponse
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string ISBN { get; set; } = string.Empty;

    public string AuthorName { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public string ShelfCode { get; set; } = string.Empty;

    public int Floor { get; set; }

    public string Section { get; set; } = string.Empty;

    public int TotalStock { get; set; }

    public int AvailableStock { get; set; }

    public bool IsAvailable { get; set; }
}

public sealed class BookDetailResponse
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string ISBN { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int PublicationYear { get; set; }

    public string? Publisher { get; set; }

    public string? ImageUrl { get; set; }

    public int TotalStock { get; set; }

    public int AvailableStock { get; set; }

    public bool IsAvailable { get; set; }

    public int AuthorId { get; set; }

    public string AuthorName { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public int ShelfId { get; set; }

    public string ShelfCode { get; set; } = string.Empty;

    public int Floor { get; set; }

    public string Section { get; set; } = string.Empty;

    public string ShelfLocation { get; set; } = string.Empty;
}