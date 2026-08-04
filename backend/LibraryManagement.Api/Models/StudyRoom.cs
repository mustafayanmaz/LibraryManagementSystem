using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Api.Models;

public class StudyRoom
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string RoomNumber { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public int Floor { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<RoomReservation> Reservations { get; set; }
        = new List<RoomReservation>();
}