using System.ComponentModel.DataAnnotations;
using LibraryManagement.Api.Enums;

namespace LibraryManagement.Api.DTOs.BookReservations;

public sealed class CreateBookReservationRequest
{
    [Range(1, int.MaxValue)]
    public int BookId { get; set; }
}

public sealed class UpdateBookReservationStatusRequest
{
    [EnumDataType(typeof(ReservationStatus))]
    public ReservationStatus Status { get; set; }
}

public sealed class BookReservationResponse
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string StudentName { get; set; } = string.Empty;

    public string? StudentNumber { get; set; }

    public int BookId { get; set; }

    public string BookTitle { get; set; } = string.Empty;

    public string ISBN { get; set; } = string.Empty;

    public string AuthorName { get; set; } = string.Empty;

    public string ShelfCode { get; set; } = string.Empty;

    public string ShelfLocation { get; set; } = string.Empty;

    public DateTime ReservationDate { get; set; }

    public DateTime ExpirationDate { get; set; }

    public ReservationStatus Status { get; set; }

    public int AvailableStock { get; set; }

    public bool CanCancel { get; set; }
}