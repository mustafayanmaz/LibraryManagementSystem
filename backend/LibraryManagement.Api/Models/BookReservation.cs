using LibraryManagement.Api.Enums;

namespace LibraryManagement.Api.Models;

public class BookReservation
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public ApplicationUser User { get; set; } = null!;

    public int BookId { get; set; }

    public Book Book { get; set; } = null!;

    public DateTime ReservationDate { get; set; } = DateTime.UtcNow;

    public DateTime ExpirationDate { get; set; }

    public ReservationStatus Status { get; set; }
        = ReservationStatus.Pending;
}