using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Api.DTOs.StudyRooms;

public sealed class StudyRoomUpsertRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(20, MinimumLength = 1)]
    public string RoomNumber { get; set; } = string.Empty;

    [Range(1, 20)]
    public int Capacity { get; set; }

    [Range(0, 20)]
    public int Floor { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}

public sealed class StudyRoomResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string RoomNumber { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public int Floor { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public int ReservationCount { get; set; }
}