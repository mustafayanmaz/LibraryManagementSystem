using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Api.Models;

public class Shelf
{
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string Code { get; set; } = string.Empty;

    public int Floor { get; set; }

    [Required]
    [MaxLength(100)]
    public string Section { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public ICollection<Book> Books { get; set; } = new List<Book>();
}