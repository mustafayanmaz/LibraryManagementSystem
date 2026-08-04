using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Api.Models;

public class Book
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(13)]
    public string ISBN { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public int PublicationYear { get; set; }

    [MaxLength(150)]
    public string? Publisher { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public int TotalStock { get; set; }

    public int AvailableStock { get; set; }

    public int AuthorId { get; set; }

    public Author Author { get; set; } = null!;

    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    public int ShelfId { get; set; }

    public Shelf Shelf { get; set; } = null!;

    public ICollection<BookReservation> Reservations { get; set; }
        = new List<BookReservation>();
}