using System.ComponentModel.DataAnnotations;
using LibraryManagement.Api.Enums;

namespace LibraryManagement.Api.DTOs.RoomReservations;

public sealed class CreateRoomReservationRequest
{
    [Range(1, int.MaxValue)]
    public int StudyRoomId { get; set; }

    public DateOnly ReservationDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }
}

public sealed class UpdateRoomReservationStatusRequest
{
    [EnumDataType(typeof(ReservationStatus))]
    public ReservationStatus Status { get; set; }
}

public sealed class RoomReservationResponse
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string StudentName { get; set; } = string.Empty;

    public string? StudentNumber { get; set; }

    public int StudyRoomId { get; set; }

    public string RoomName { get; set; } = string.Empty;

    public string RoomNumber { get; set; } = string.Empty;

    public int Floor { get; set; }

    public DateOnly ReservationDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public ReservationStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool CanCancel { get; set; }
}