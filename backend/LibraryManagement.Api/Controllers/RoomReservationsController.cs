using System.Data;
using System.Linq.Expressions;
using System.Security.Claims;
using LibraryManagement.Api.Constants;
using LibraryManagement.Api.Data;
using LibraryManagement.Api.DTOs.RoomReservations;
using LibraryManagement.Api.Enums;
using LibraryManagement.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace LibraryManagement.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/room-reservations")]
public class RoomReservationsController : ControllerBase
{
    private static readonly TimeOnly OpeningTime = new(8, 0);
    private static readonly TimeOnly ClosingTime = new(22, 0);
    private const int MaximumDurationMinutes = 120;

    private readonly ApplicationDbContext _dbContext;

    public RoomReservationsController(
        ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("my")]
    public async Task<ActionResult<
        IReadOnlyList<RoomReservationResponse>>>
        GetMyReservations(
            [FromQuery] ReservationStatus? status)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var query = _dbContext.RoomReservations
            .AsNoTracking()
            .Where(reservation =>
                reservation.UserId == userId);

        if (status.HasValue)
        {
            query = query.Where(reservation =>
                reservation.Status == status.Value);
        }

        var reservations = await query
            .OrderByDescending(reservation =>
                reservation.ReservationDate)
            .ThenBy(reservation =>
                reservation.StartTime)
            .Select(ReservationProjection)
            .ToListAsync();

        return Ok(reservations);
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpGet]
    public async Task<ActionResult<
        IReadOnlyList<RoomReservationResponse>>> GetAll(
        [FromQuery] ReservationStatus? status,
        [FromQuery] DateOnly? date,
        [FromQuery] int? roomId,
        [FromQuery] string? search)
    {
        var query = _dbContext.RoomReservations
            .AsNoTracking()
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(reservation =>
                reservation.Status == status.Value);
        }

        if (date.HasValue)
        {
            query = query.Where(reservation =>
                reservation.ReservationDate == date.Value);
        }

        if (roomId.HasValue)
        {
            query = query.Where(reservation =>
                reservation.StudyRoomId == roomId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";

            query = query.Where(reservation =>
                EF.Functions.ILike(
                    reservation.StudyRoom.Name,
                    pattern) ||
                EF.Functions.ILike(
                    reservation.StudyRoom.RoomNumber,
                    pattern) ||
                EF.Functions.ILike(
                    reservation.User.FirstName,
                    pattern) ||
                EF.Functions.ILike(
                    reservation.User.LastName,
                    pattern) ||
                (
                    reservation.User.StudentNumber != null &&
                    EF.Functions.ILike(
                        reservation.User.StudentNumber,
                        pattern)
                ));
        }

        var reservations = await query
            .OrderByDescending(reservation =>
                reservation.ReservationDate)
            .ThenBy(reservation =>
                reservation.StartTime)
            .Select(ReservationProjection)
            .ToListAsync();

        return Ok(reservations);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<
        RoomReservationResponse>> GetById(int id)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var query = _dbContext.RoomReservations
            .AsNoTracking()
            .Where(reservation =>
                reservation.Id == id);

        if (!User.IsInRole(UserRoles.Admin))
        {
            query = query.Where(reservation =>
                reservation.UserId == userId);
        }

        var reservation = await query
            .Select(ReservationProjection)
            .FirstOrDefaultAsync();

        if (reservation is null)
        {
            return NotFound(new
            {
                message = "Oda rezervasyonu bulunamadı."
            });
        }

        return Ok(reservation);
    }

    [HttpPost]
    public async Task<ActionResult<
        RoomReservationResponse>> Create(
        CreateRoomReservationRequest request)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var validationError = ValidateTimeRange(request);

        if (validationError is not null)
        {
            return BadRequest(new
            {
                message = validationError
            });
        }

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        try
        {
            var room = await _dbContext.StudyRooms
                .AsNoTracking()
                .FirstOrDefaultAsync(item =>
                    item.Id == request.StudyRoomId);

            if (room is null)
            {
                return NotFound(new
                {
                    message = "Çalışma odası bulunamadı."
                });
            }

            if (!room.IsActive)
            {
                return Conflict(new
                {
                    message =
                        "Seçilen çalışma odası kullanıma kapalı."
                });
            }

            var hasRoomConflict =
                await _dbContext.RoomReservations.AnyAsync(
                    reservation =>
                        reservation.StudyRoomId ==
                            request.StudyRoomId &&
                        reservation.ReservationDate ==
                            request.ReservationDate &&
                        (
                            reservation.Status ==
                                ReservationStatus.Pending ||
                            reservation.Status ==
                                ReservationStatus.Approved
                        ) &&
                        request.StartTime <
                            reservation.EndTime &&
                        request.EndTime >
                            reservation.StartTime);

            if (hasRoomConflict)
            {
                return Conflict(new
                {
                    message =
                        "Oda seçilen tarih ve saatlerde dolu."
                });
            }

            var hasUserConflict =
                await _dbContext.RoomReservations.AnyAsync(
                    reservation =>
                        reservation.UserId == userId &&
                        reservation.ReservationDate ==
                            request.ReservationDate &&
                        (
                            reservation.Status ==
                                ReservationStatus.Pending ||
                            reservation.Status ==
                                ReservationStatus.Approved
                        ) &&
                        request.StartTime <
                            reservation.EndTime &&
                        request.EndTime >
                            reservation.StartTime);

            if (hasUserConflict)
            {
                return Conflict(new
                {
                    message =
                        "Aynı saat aralığında başka bir " +
                        "oda rezervasyonunuz bulunuyor."
                });
            }

            var reservation = new RoomReservation
            {
                UserId = userId,
                StudyRoomId = request.StudyRoomId,
                ReservationDate =
                    request.ReservationDate,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Status = ReservationStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.RoomReservations.Add(reservation);

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            var response =
                await GetResponseAsync(reservation.Id);

            return CreatedAtAction(
                nameof(GetById),
                new { id = reservation.Id },
                response);
        }
        catch (PostgresException exception)
            when (
                exception.SqlState ==
                PostgresErrorCodes.SerializationFailure)
        {
            return Conflict(new
            {
                message =
                    "Aynı zaman aralığında başka bir işlem " +
                    "yapıldı. Uygunluğu tekrar kontrol edin."
            });
        }
        catch (DbUpdateException exception)
            when (
                exception.InnerException is PostgresException
                {
                    SqlState:
                        PostgresErrorCodes.SerializationFailure
                })
        {
            return Conflict(new
            {
                message =
                    "Aynı zaman aralığında başka bir işlem " +
                    "yapıldı. Uygunluğu tekrar kontrol edin."
            });
        }
    }

    [HttpPut("{id:int}/cancel")]
    public async Task<ActionResult<
        RoomReservationResponse>> Cancel(int id)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var reservation =
            await _dbContext.RoomReservations
                .FirstOrDefaultAsync(item =>
                    item.Id == id &&
                    item.UserId == userId);

        if (reservation is null)
        {
            return NotFound(new
            {
                message = "Oda rezervasyonu bulunamadı."
            });
        }

        if (!IsActive(reservation.Status))
        {
            return Conflict(new
            {
                message =
                    "Yalnızca aktif rezervasyonlar iptal edilebilir."
            });
        }

        reservation.Status = ReservationStatus.Cancelled;

        await _dbContext.SaveChangesAsync();

        return Ok(await GetResponseAsync(id));
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPut("{id:int}/status")]
    public async Task<ActionResult<
        RoomReservationResponse>> UpdateStatus(
        int id,
        UpdateRoomReservationStatusRequest request)
    {
        var reservation =
            await _dbContext.RoomReservations
                .FirstOrDefaultAsync(item =>
                    item.Id == id);

        if (reservation is null)
        {
            return NotFound(new
            {
                message = "Oda rezervasyonu bulunamadı."
            });
        }

        if (reservation.Status == request.Status)
        {
            return Ok(await GetResponseAsync(id));
        }

        if (!IsValidTransition(
                reservation.Status,
                request.Status))
        {
            return Conflict(new
            {
                message =
                    $"{reservation.Status} durumundan " +
                    $"{request.Status} durumuna geçilemez."
            });
        }

        reservation.Status = request.Status;

        await _dbContext.SaveChangesAsync();

        return Ok(await GetResponseAsync(id));
    }

    private string? GetCurrentUserId()
    {
        return User.FindFirstValue(
            ClaimTypes.NameIdentifier);
    }

    private async Task<RoomReservationResponse?>
        GetResponseAsync(int id)
    {
        return await _dbContext.RoomReservations
            .AsNoTracking()
            .Where(reservation =>
                reservation.Id == id)
            .Select(ReservationProjection)
            .FirstOrDefaultAsync();
    }

    private static string? ValidateTimeRange(
        CreateRoomReservationRequest request)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        if (request.ReservationDate < today)
        {
            return "Geçmiş bir tarih için rezervasyon yapılamaz.";
        }

        if (request.StartTime >= request.EndTime)
        {
            return "Bitiş saati başlangıç saatinden sonra olmalıdır.";
        }

        if (request.StartTime < OpeningTime ||
            request.EndTime > ClosingTime)
        {
            return "Rezervasyonlar 08.00 ile 22.00 arasında yapılabilir.";
        }

        var duration =
            request.EndTime.ToTimeSpan() -
            request.StartTime.ToTimeSpan();

        if (duration.TotalMinutes > MaximumDurationMinutes)
        {
            return "Bir rezervasyon en fazla iki saat olabilir.";
        }

        if (request.ReservationDate == today &&
            request.StartTime <=
                TimeOnly.FromDateTime(DateTime.Now))
        {
            return "Geçmiş bir saat için rezervasyon yapılamaz.";
        }

        return null;
    }

    private static bool IsActive(
        ReservationStatus status)
    {
        return status is
            ReservationStatus.Pending or
            ReservationStatus.Approved;
    }

    private static bool IsValidTransition(
        ReservationStatus currentStatus,
        ReservationStatus newStatus)
    {
        return currentStatus switch
        {
            ReservationStatus.Pending =>
                newStatus is
                    ReservationStatus.Approved or
                    ReservationStatus.Cancelled or
                    ReservationStatus.Expired,

            ReservationStatus.Approved =>
                newStatus is
                    ReservationStatus.Cancelled or
                    ReservationStatus.Completed or
                    ReservationStatus.Expired,

            _ => false
        };
    }

    private static readonly Expression<
        Func<RoomReservation, RoomReservationResponse>>
        ReservationProjection = reservation =>
            new RoomReservationResponse
            {
                Id = reservation.Id,
                UserId = reservation.UserId,
                StudentName =
                    reservation.User.FirstName + " " +
                    reservation.User.LastName,
                StudentNumber =
                    reservation.User.StudentNumber,
                StudyRoomId =
                    reservation.StudyRoomId,
                RoomName =
                    reservation.StudyRoom.Name,
                RoomNumber =
                    reservation.StudyRoom.RoomNumber,
                Floor =
                    reservation.StudyRoom.Floor,
                ReservationDate =
                    reservation.ReservationDate,
                StartTime =
                    reservation.StartTime,
                EndTime =
                    reservation.EndTime,
                Status =
                    reservation.Status,
                CreatedAt =
                    reservation.CreatedAt,
                CanCancel =
                    reservation.Status ==
                        ReservationStatus.Pending ||
                    reservation.Status ==
                        ReservationStatus.Approved
            };
}