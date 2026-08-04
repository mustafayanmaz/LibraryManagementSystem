using LibraryManagement.Api.Constants;
using LibraryManagement.Api.Data;
using LibraryManagement.Api.DTOs.StudyRooms;
using LibraryManagement.Api.Enums;
using LibraryManagement.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Api.Controllers;

[ApiController]
[Route("api/study-rooms")]
public class StudyRoomsController : ControllerBase
{
    private static readonly TimeOnly OpeningTime = new(8, 0);
    private static readonly TimeOnly ClosingTime = new(22, 0);
    private const int MaximumDurationMinutes = 120;

    private readonly ApplicationDbContext _dbContext;

    public StudyRoomsController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<
        IReadOnlyList<StudyRoomResponse>>> GetAll(
        [FromQuery] bool activeOnly = false)
    {
        var query = _dbContext.StudyRooms
            .AsNoTracking()
            .AsQueryable();

        if (activeOnly)
        {
            query = query.Where(room => room.IsActive);
        }

        var rooms = await query
            .OrderBy(room => room.Floor)
            .ThenBy(room => room.RoomNumber)
            .Select(room => new StudyRoomResponse
            {
                Id = room.Id,
                Name = room.Name,
                RoomNumber = room.RoomNumber,
                Capacity = room.Capacity,
                Floor = room.Floor,
                Description = room.Description,
                IsActive = room.IsActive,
                ReservationCount = room.Reservations.Count
            })
            .ToListAsync();

        return Ok(rooms);
    }

    [Authorize]
    [HttpGet("available")]
    public async Task<ActionResult<
        IReadOnlyList<StudyRoomResponse>>> GetAvailable(
        [FromQuery] DateOnly date,
        [FromQuery] TimeOnly startTime,
        [FromQuery] TimeOnly endTime)
    {
        var validationError = ValidateTimeRange(
            date,
            startTime,
            endTime);

        if (validationError is not null)
        {
            return BadRequest(new
            {
                message = validationError
            });
        }

        var rooms = await _dbContext.StudyRooms
            .AsNoTracking()
            .Where(room =>
                room.IsActive &&
                !room.Reservations.Any(reservation =>
                    reservation.ReservationDate == date &&
                    (
                        reservation.Status ==
                            ReservationStatus.Pending ||
                        reservation.Status ==
                            ReservationStatus.Approved
                    ) &&
                    startTime < reservation.EndTime &&
                    endTime > reservation.StartTime))
            .OrderBy(room => room.Floor)
            .ThenBy(room => room.RoomNumber)
            .Select(room => new StudyRoomResponse
            {
                Id = room.Id,
                Name = room.Name,
                RoomNumber = room.RoomNumber,
                Capacity = room.Capacity,
                Floor = room.Floor,
                Description = room.Description,
                IsActive = room.IsActive,
                ReservationCount = room.Reservations.Count
            })
            .ToListAsync();

        return Ok(rooms);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<StudyRoomResponse>> GetById(
        int id)
    {
        var room = await _dbContext.StudyRooms
            .AsNoTracking()
            .Where(room => room.Id == id)
            .Select(room => new StudyRoomResponse
            {
                Id = room.Id,
                Name = room.Name,
                RoomNumber = room.RoomNumber,
                Capacity = room.Capacity,
                Floor = room.Floor,
                Description = room.Description,
                IsActive = room.IsActive,
                ReservationCount = room.Reservations.Count
            })
            .FirstOrDefaultAsync();

        if (room is null)
        {
            return NotFound(new
            {
                message = "Çalışma odası bulunamadı."
            });
        }

        return Ok(room);
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost]
    public async Task<ActionResult<StudyRoomResponse>> Create(
        StudyRoomUpsertRequest request)
    {
        var roomNumber = NormalizeRoomNumber(
            request.RoomNumber);

        var roomNumberExists =
            await _dbContext.StudyRooms.AnyAsync(
                room => room.RoomNumber == roomNumber);

        if (roomNumberExists)
        {
            return Conflict(new
            {
                message =
                    "Bu oda numarasıyla kayıtlı bir oda bulunuyor."
            });
        }

        var room = new StudyRoom
        {
            Name = request.Name.Trim(),
            RoomNumber = roomNumber,
            Capacity = request.Capacity,
            Floor = request.Floor,
            Description = NormalizeOptional(
                request.Description),
            IsActive = request.IsActive
        };

        _dbContext.StudyRooms.Add(room);
        await _dbContext.SaveChangesAsync();

        var response = new StudyRoomResponse
        {
            Id = room.Id,
            Name = room.Name,
            RoomNumber = room.RoomNumber,
            Capacity = room.Capacity,
            Floor = room.Floor,
            Description = room.Description,
            IsActive = room.IsActive,
            ReservationCount = 0
        };

        return CreatedAtAction(
            nameof(GetById),
            new { id = room.Id },
            response);
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        StudyRoomUpsertRequest request)
    {
        var room = await _dbContext.StudyRooms.FindAsync(id);

        if (room is null)
        {
            return NotFound(new
            {
                message = "Çalışma odası bulunamadı."
            });
        }

        var roomNumber = NormalizeRoomNumber(
            request.RoomNumber);

        var roomNumberExists =
            await _dbContext.StudyRooms.AnyAsync(
                existingRoom =>
                    existingRoom.Id != id &&
                    existingRoom.RoomNumber == roomNumber);

        if (roomNumberExists)
        {
            return Conflict(new
            {
                message =
                    "Bu oda numarası başka bir odada kullanılıyor."
            });
        }

        room.Name = request.Name.Trim();
        room.RoomNumber = roomNumber;
        room.Capacity = request.Capacity;
        room.Floor = request.Floor;
        room.Description = NormalizeOptional(
            request.Description);
        room.IsActive = request.IsActive;

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var room = await _dbContext.StudyRooms.FindAsync(id);

        if (room is null)
        {
            return NotFound(new
            {
                message = "Çalışma odası bulunamadı."
            });
        }

        var hasReservations =
            await _dbContext.RoomReservations.AnyAsync(
                reservation =>
                    reservation.StudyRoomId == id);

        if (hasReservations)
        {
            return Conflict(new
            {
                message =
                    "Rezervasyon geçmişi bulunan oda silinemez. " +
                    "Odayı pasif hale getirebilirsiniz."
            });
        }

        _dbContext.StudyRooms.Remove(room);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    private static string? ValidateTimeRange(
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        if (date < today)
        {
            return "Geçmiş bir tarih için rezervasyon yapılamaz.";
        }

        if (startTime >= endTime)
        {
            return "Bitiş saati başlangıç saatinden sonra olmalıdır.";
        }

        if (startTime < OpeningTime || endTime > ClosingTime)
        {
            return "Rezervasyonlar 08.00 ile 22.00 arasında yapılabilir.";
        }

        var duration = endTime.ToTimeSpan() -
                       startTime.ToTimeSpan();

        if (duration.TotalMinutes > MaximumDurationMinutes)
        {
            return "Bir rezervasyon en fazla iki saat olabilir.";
        }

        if (date == today &&
            startTime <= TimeOnly.FromDateTime(DateTime.Now))
        {
            return "Geçmiş bir saat için rezervasyon yapılamaz.";
        }

        return null;
    }

    private static string NormalizeRoomNumber(string value)
    {
        return value.Trim().ToUpperInvariant();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}