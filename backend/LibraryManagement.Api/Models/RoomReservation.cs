using LibraryManagement.Api.Enums;

namespace LibraryManagement.Api.Models;

public class RoomReservation
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public ApplicationUser User { get; set; } = null!;

    public int StudyRoomId { get; set; }

    public StudyRoom StudyRoom { get; set; } = null!;

    public DateOnly ReservationDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public ReservationStatus Status { get; set; }
        = ReservationStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}