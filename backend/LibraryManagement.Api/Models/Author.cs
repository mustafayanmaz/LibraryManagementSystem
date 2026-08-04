using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Api.Models;

public class Author
{
    public int Id { get; set; }

    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Biography { get; set; }

    public ICollection<Book> Books { get; set; } = new List<Book>();
}