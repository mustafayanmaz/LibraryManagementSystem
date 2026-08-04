using System.Linq.Expressions;
using System.Security.Claims;
using LibraryManagement.Api.Constants;
using LibraryManagement.Api.Data;
using LibraryManagement.Api.DTOs.BookReservations;
using LibraryManagement.Api.Enums;
using LibraryManagement.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace LibraryManagement.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/book-reservations")]
public class BookReservationsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public BookReservationsController(
        ApplicationDbContext dbContext,
        IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    [HttpGet("my")]
    public async Task<ActionResult<
        IReadOnlyList<BookReservationResponse>>> GetMyReservations(
        [FromQuery] ReservationStatus? status)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var query = _dbContext.BookReservations
            .AsNoTracking()
            .Where(reservation => reservation.UserId == userId);

        if (status.HasValue)
        {
            query = query.Where(
                reservation =>
                    reservation.Status == status.Value);
        }

        var reservations = await query
            .OrderByDescending(
                reservation => reservation.ReservationDate)
            .Select(ReservationProjection)
            .ToListAsync();

        return Ok(reservations);
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpGet]
    public async Task<ActionResult<
        IReadOnlyList<BookReservationResponse>>> GetAll(
        [FromQuery] ReservationStatus? status,
        [FromQuery] string? search)
    {
        var query = _dbContext.BookReservations
            .AsNoTracking()
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(
                reservation =>
                    reservation.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";

            query = query.Where(reservation =>
                EF.Functions.ILike(
                    reservation.Book.Title,
                    pattern) ||
                EF.Functions.ILike(
                    reservation.User.FirstName,
                    pattern) ||
                EF.Functions.ILike(
                    reservation.User.LastName,
                    pattern) ||
                (
                    reservation.User.Email != null &&
                    EF.Functions.ILike(
                        reservation.User.Email,
                        pattern)
                ) ||
                (
                    reservation.User.StudentNumber != null &&
                    EF.Functions.ILike(
                        reservation.User.StudentNumber,
                        pattern)
                ));
        }

        var reservations = await query
            .OrderByDescending(
                reservation => reservation.ReservationDate)
            .Select(ReservationProjection)
            .ToListAsync();

        return Ok(reservations);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<
        BookReservationResponse>> GetById(int id)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var query = _dbContext.BookReservations
            .AsNoTracking()
            .Where(reservation => reservation.Id == id);

        if (!User.IsInRole(UserRoles.Admin))
        {
            query = query.Where(
                reservation => reservation.UserId == userId);
        }

        var reservation = await query
            .Select(ReservationProjection)
            .FirstOrDefaultAsync();

        if (reservation is null)
        {
            return NotFound(new
            {
                message = "Rezervasyon bulunamadı."
            });
        }

        return Ok(reservation);
    }

    [HttpPost]
    public async Task<ActionResult<
        BookReservationResponse>> Create(
        CreateBookReservationRequest request)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var bookExists = await _dbContext.Books
            .AsNoTracking()
            .AnyAsync(book => book.Id == request.BookId);

        if (!bookExists)
        {
            return NotFound(new
            {
                message = "Kitap bulunamadı."
            });
        }

        var hasActiveReservation =
            await _dbContext.BookReservations.AnyAsync(
                reservation =>
                    reservation.UserId == userId &&
                    reservation.BookId == request.BookId &&
                    (
                        reservation.Status ==
                            ReservationStatus.Pending ||
                        reservation.Status ==
                            ReservationStatus.Approved
                    ));

        if (hasActiveReservation)
        {
            return Conflict(new
            {
                message =
                    "Bu kitap için zaten aktif bir rezervasyonunuz var."
            });
        }

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync();

        var affectedBookCount = await _dbContext.Books
            .Where(book =>
                book.Id == request.BookId &&
                book.AvailableStock > 0)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(
                    book => book.AvailableStock,
                    book => book.AvailableStock - 1));

        if (affectedBookCount == 0)
        {
            await transaction.RollbackAsync();

            return Conflict(new
            {
                message =
                    "Kitabın kullanılabilir stoğu bulunmuyor."
            });
        }

        var expiryDays = _configuration.GetValue<int?>(
            "ReservationSettings:BookReservationExpiryDays") ?? 3;

        var reservationDate = DateTime.UtcNow;

        var reservation = new BookReservation
        {
            UserId = userId,
            BookId = request.BookId,
            ReservationDate = reservationDate,
            ExpirationDate =
                reservationDate.AddDays(expiryDays),
            Status = ReservationStatus.Pending
        };

        _dbContext.BookReservations.Add(reservation);

        try
        {
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (DbUpdateException exception)
            when (
                exception.InnerException is PostgresException
                {
                    SqlState:
                        PostgresErrorCodes.UniqueViolation
                })
        {
            await transaction.RollbackAsync();

            return Conflict(new
            {
                message =
                    "Bu kitap için zaten aktif bir rezervasyonunuz var."
            });
        }

        var response = await GetResponseAsync(reservation.Id);

        return CreatedAtAction(
            nameof(GetById),
            new { id = reservation.Id },
            response);
    }

    [HttpPut("{id:int}/cancel")]
    public async Task<ActionResult<
        BookReservationResponse>> Cancel(int id)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var reservation = await _dbContext.BookReservations
            .Include(item => item.Book)
            .FirstOrDefaultAsync(item =>
                item.Id == id &&
                item.UserId == userId);

        if (reservation is null)
        {
            return NotFound(new
            {
                message = "Rezervasyon bulunamadı."
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

        reservation.Book.AvailableStock = Math.Min(
            reservation.Book.TotalStock,
            reservation.Book.AvailableStock + 1);

        await _dbContext.SaveChangesAsync();

        return Ok(await GetResponseAsync(reservation.Id));
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPut("{id:int}/status")]
    public async Task<ActionResult<
        BookReservationResponse>> UpdateStatus(
        int id,
        UpdateBookReservationStatusRequest request)
    {
        var reservation = await _dbContext.BookReservations
            .Include(item => item.Book)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (reservation is null)
        {
            return NotFound(new
            {
                message = "Rezervasyon bulunamadı."
            });
        }

        if (reservation.Status == request.Status)
        {
            return Ok(await GetResponseAsync(reservation.Id));
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

        if (IsActive(reservation.Status) &&
            !IsActive(request.Status))
        {
            reservation.Book.AvailableStock = Math.Min(
                reservation.Book.TotalStock,
                reservation.Book.AvailableStock + 1);
        }

        reservation.Status = request.Status;

        await _dbContext.SaveChangesAsync();

        return Ok(await GetResponseAsync(reservation.Id));
    }

    private string? GetCurrentUserId()
    {
        return User.FindFirstValue(
            ClaimTypes.NameIdentifier);
    }

    private async Task<BookReservationResponse?>
        GetResponseAsync(int id)
    {
        return await _dbContext.BookReservations
            .AsNoTracking()
            .Where(reservation => reservation.Id == id)
            .Select(ReservationProjection)
            .FirstOrDefaultAsync();
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
        Func<BookReservation, BookReservationResponse>>
        ReservationProjection = reservation =>
            new BookReservationResponse
            {
                Id = reservation.Id,
                UserId = reservation.UserId,
                StudentName =
                    reservation.User.FirstName + " " +
                    reservation.User.LastName,
                StudentNumber =
                    reservation.User.StudentNumber,
                BookId = reservation.BookId,
                BookTitle = reservation.Book.Title,
                ISBN = reservation.Book.ISBN,
                AuthorName =
                    reservation.Book.Author.Name,
                ShelfCode =
                    reservation.Book.Shelf.Code,
                ShelfLocation =
                    reservation.Book.Shelf.Floor +
                    ". Kat - " +
                    reservation.Book.Shelf.Section +
                    " - " +
                    reservation.Book.Shelf.Code,
                ReservationDate =
                    reservation.ReservationDate,
                ExpirationDate =
                    reservation.ExpirationDate,
                Status = reservation.Status,
                AvailableStock =
                    reservation.Book.AvailableStock,
                CanCancel =
                    reservation.Status ==
                        ReservationStatus.Pending ||
                    reservation.Status ==
                        ReservationStatus.Approved
            };
}