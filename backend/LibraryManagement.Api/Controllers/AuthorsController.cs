using LibraryManagement.Api.Constants;
using LibraryManagement.Api.Data;
using LibraryManagement.Api.DTOs.Authors;
using LibraryManagement.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Api.Controllers;

[ApiController]
[Route("api/authors")]
public class AuthorsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public AuthorsController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<
        IReadOnlyList<AuthorResponse>>> GetAll()
    {
        var authors = await _dbContext.Authors
            .AsNoTracking()
            .OrderBy(author => author.Name)
            .Select(author => new AuthorResponse
            {
                Id = author.Id,
                Name = author.Name,
                Biography = author.Biography,
                BookCount = author.Books.Count
            })
            .ToListAsync();

        return Ok(authors);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AuthorResponse>> GetById(
        int id)
    {
        var author = await _dbContext.Authors
            .AsNoTracking()
            .Where(author => author.Id == id)
            .Select(author => new AuthorResponse
            {
                Id = author.Id,
                Name = author.Name,
                Biography = author.Biography,
                BookCount = author.Books.Count
            })
            .FirstOrDefaultAsync();

        if (author is null)
        {
            return NotFound(new
            {
                message = "Yazar bulunamadı."
            });
        }

        return Ok(author);
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost]
    public async Task<ActionResult<AuthorResponse>> Create(
        AuthorUpsertRequest request)
    {
        var name = request.Name.Trim();

        var exists = await _dbContext.Authors.AnyAsync(
            author => EF.Functions.ILike(author.Name, name));

        if (exists)
        {
            return Conflict(new
            {
                message = "Bu isimde bir yazar zaten bulunuyor."
            });
        }

        var author = new Author
        {
            Name = name,
            Biography = NormalizeOptional(request.Biography)
        };

        _dbContext.Authors.Add(author);
        await _dbContext.SaveChangesAsync();

        var response = new AuthorResponse
        {
            Id = author.Id,
            Name = author.Name,
            Biography = author.Biography,
            BookCount = 0
        };

        return CreatedAtAction(
            nameof(GetById),
            new { id = author.Id },
            response);
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        AuthorUpsertRequest request)
    {
        var author = await _dbContext.Authors.FindAsync(id);

        if (author is null)
        {
            return NotFound(new
            {
                message = "Yazar bulunamadı."
            });
        }

        var name = request.Name.Trim();

        var exists = await _dbContext.Authors.AnyAsync(
            existingAuthor =>
                existingAuthor.Id != id &&
                EF.Functions.ILike(existingAuthor.Name, name));

        if (exists)
        {
            return Conflict(new
            {
                message =
                    "Bu isimde başka bir yazar zaten bulunuyor."
            });
        }

        author.Name = name;
        author.Biography = NormalizeOptional(
            request.Biography);

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var author = await _dbContext.Authors.FindAsync(id);

        if (author is null)
        {
            return NotFound(new
            {
                message = "Yazar bulunamadı."
            });
        }

        var isUsed = await _dbContext.Books.AnyAsync(
            book => book.AuthorId == id);

        if (isUsed)
        {
            return Conflict(new
            {
                message =
                    "Kitap kaydı bulunan yazar silinemez."
            });
        }

        _dbContext.Authors.Remove(author);
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