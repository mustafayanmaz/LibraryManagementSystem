using LibraryManagement.Api.Constants;
using LibraryManagement.Api.Data;
using LibraryManagement.Api.DTOs.Shelves;
using LibraryManagement.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Api.Controllers;

[ApiController]
[Route("api/shelves")]
public class ShelvesController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public ShelvesController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<
        IReadOnlyList<ShelfResponse>>> GetAll()
    {
        var shelves = await _dbContext.Shelves
            .AsNoTracking()
            .OrderBy(shelf => shelf.Floor)
            .ThenBy(shelf => shelf.Code)
            .Select(shelf => new ShelfResponse
            {
                Id = shelf.Id,
                Code = shelf.Code,
                Floor = shelf.Floor,
                Section = shelf.Section,
                Description = shelf.Description,
                Location =
                    $"{shelf.Floor}. Kat - " +
                    $"{shelf.Section} - {shelf.Code}",
                BookCount = shelf.Books.Count
            })
            .ToListAsync();

        return Ok(shelves);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ShelfResponse>> GetById(
        int id)
    {
        var shelf = await _dbContext.Shelves
            .AsNoTracking()
            .Where(shelf => shelf.Id == id)
            .Select(shelf => new ShelfResponse
            {
                Id = shelf.Id,
                Code = shelf.Code,
                Floor = shelf.Floor,
                Section = shelf.Section,
                Description = shelf.Description,
                Location =
                    $"{shelf.Floor}. Kat - " +
                    $"{shelf.Section} - {shelf.Code}",
                BookCount = shelf.Books.Count
            })
            .FirstOrDefaultAsync();

        if (shelf is null)
        {
            return NotFound(new
            {
                message = "Raf bulunamadı."
            });
        }

        return Ok(shelf);
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost]
    public async Task<ActionResult<ShelfResponse>> Create(
        ShelfUpsertRequest request)
    {
        var code = request.Code.Trim().ToUpperInvariant();

        var exists = await _dbContext.Shelves.AnyAsync(
            shelf => shelf.Code == code);

        if (exists)
        {
            return Conflict(new
            {
                message = "Bu raf kodu zaten kullanılıyor."
            });
        }

        var shelf = new Shelf
        {
            Code = code,
            Floor = request.Floor,
            Section = request.Section.Trim(),
            Description = NormalizeOptional(
                request.Description)
        };

        _dbContext.Shelves.Add(shelf);
        await _dbContext.SaveChangesAsync();

        var response = new ShelfResponse
        {
            Id = shelf.Id,
            Code = shelf.Code,
            Floor = shelf.Floor,
            Section = shelf.Section,
            Description = shelf.Description,
            Location =
                $"{shelf.Floor}. Kat - " +
                $"{shelf.Section} - {shelf.Code}",
            BookCount = 0
        };

        return CreatedAtAction(
            nameof(GetById),
            new { id = shelf.Id },
            response);
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        ShelfUpsertRequest request)
    {
        var shelf = await _dbContext.Shelves.FindAsync(id);

        if (shelf is null)
        {
            return NotFound(new
            {
                message = "Raf bulunamadı."
            });
        }

        var code = request.Code.Trim().ToUpperInvariant();

        var exists = await _dbContext.Shelves.AnyAsync(
            existingShelf =>
                existingShelf.Id != id &&
                existingShelf.Code == code);

        if (exists)
        {
            return Conflict(new
            {
                message =
                    "Bu raf kodu başka bir rafta kullanılıyor."
            });
        }

        shelf.Code = code;
        shelf.Floor = request.Floor;
        shelf.Section = request.Section.Trim();
        shelf.Description = NormalizeOptional(
            request.Description);

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var shelf = await _dbContext.Shelves.FindAsync(id);

        if (shelf is null)
        {
            return NotFound(new
            {
                message = "Raf bulunamadı."
            });
        }

        var isUsed = await _dbContext.Books.AnyAsync(
            book => book.ShelfId == id);

        if (isUsed)
        {
            return Conflict(new
            {
                message =
                    "Kitap kaydı bulunan raf silinemez."
            });
        }

        _dbContext.Shelves.Remove(shelf);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}