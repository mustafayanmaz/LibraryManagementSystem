using LibraryManagement.Api.Constants;
using LibraryManagement.Api.Data;
using LibraryManagement.Api.DTOs.Books;
using LibraryManagement.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Api.Controllers;

[ApiController]
[Route("api/books")]
public class BooksController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public BooksController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<
        IReadOnlyList<BookListItemResponse>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] int? categoryId,
        [FromQuery] int? authorId,
        [FromQuery] int? shelfId,
        [FromQuery] bool? availableOnly)
    {
        var query = _dbContext.Books
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";

            query = query.Where(book =>
                EF.Functions.ILike(book.Title, pattern) ||
                EF.Functions.ILike(book.ISBN, pattern) ||
                EF.Functions.ILike(book.Author.Name, pattern));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(
                book => book.CategoryId == categoryId.Value);
        }

        if (authorId.HasValue)
        {
            query = query.Where(
                book => book.AuthorId == authorId.Value);
        }

        if (shelfId.HasValue)
        {
            query = query.Where(
                book => book.ShelfId == shelfId.Value);
        }

        if (availableOnly == true)
        {
            query = query.Where(
                book => book.AvailableStock > 0);
        }

        var books = await query
            .OrderBy(book => book.Title)
            .Select(book => new BookListItemResponse
            {
                Id = book.Id,
                Title = book.Title,
                ISBN = book.ISBN,
                AuthorName = book.Author.Name,
                CategoryName = book.Category.Name,
                ShelfCode = book.Shelf.Code,
                Floor = book.Shelf.Floor,
                Section = book.Shelf.Section,
                TotalStock = book.TotalStock,
                AvailableStock = book.AvailableStock,
                IsAvailable = book.AvailableStock > 0
            })
            .ToListAsync();

        return Ok(books);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookDetailResponse>> GetById(
        int id)
    {
        var book = await GetBookDetailAsync(id);

        if (book is null)
        {
            return NotFound(new
            {
                message = "Kitap bulunamadı."
            });
        }

        return Ok(book);
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost]
    public async Task<ActionResult<BookDetailResponse>> Create(
        CreateBookRequest request)
    {
        var isbn = NormalizeIsbn(request.ISBN);

        var isbnExists = await _dbContext.Books.AnyAsync(
            book => book.ISBN == isbn);

        if (isbnExists)
        {
            return Conflict(new
            {
                message = "Bu ISBN numarasıyla kayıtlı bir kitap var."
            });
        }

        var relationError = await ValidateRelationsAsync(
            request.AuthorId,
            request.CategoryId,
            request.ShelfId);

        if (relationError is not null)
        {
            return BadRequest(new
            {
                message = relationError
            });
        }

        if (request.AvailableStock > request.TotalStock)
        {
            return BadRequest(new
            {
                message =
                    "Kullanılabilir stok toplam stoktan fazla olamaz."
            });
        }

        var book = new Book
        {
            Title = request.Title.Trim(),
            ISBN = isbn,
            Description = NormalizeOptional(
                request.Description),
            PublicationYear = request.PublicationYear,
            Publisher = NormalizeOptional(request.Publisher),
            ImageUrl = NormalizeOptional(request.ImageUrl),
            TotalStock = request.TotalStock,
            AvailableStock = request.AvailableStock,
            AuthorId = request.AuthorId,
            CategoryId = request.CategoryId,
            ShelfId = request.ShelfId
        };

        _dbContext.Books.Add(book);
        await _dbContext.SaveChangesAsync();

        var response = await GetBookDetailAsync(book.Id);

        return CreatedAtAction(
            nameof(GetById),
            new { id = book.Id },
            response);
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateBookRequest request)
    {
        var book = await _dbContext.Books.FindAsync(id);

        if (book is null)
        {
            return NotFound(new
            {
                message = "Kitap bulunamadı."
            });
        }

        var isbn = NormalizeIsbn(request.ISBN);

        var isbnExists = await _dbContext.Books.AnyAsync(
            existingBook =>
                existingBook.Id != id &&
                existingBook.ISBN == isbn);

        if (isbnExists)
        {
            return Conflict(new
            {
                message =
                    "Bu ISBN numarası başka bir kitapta kullanılıyor."
            });
        }

        var relationError = await ValidateRelationsAsync(
            request.AuthorId,
            request.CategoryId,
            request.ShelfId);

        if (relationError is not null)
        {
            return BadRequest(new
            {
                message = relationError
            });
        }

        if (request.AvailableStock > request.TotalStock)
        {
            return BadRequest(new
            {
                message =
                    "Kullanılabilir stok toplam stoktan fazla olamaz."
            });
        }

        book.Title = request.Title.Trim();
        book.ISBN = isbn;
        book.Description = NormalizeOptional(
            request.Description);
        book.PublicationYear = request.PublicationYear;
        book.Publisher = NormalizeOptional(request.Publisher);
        book.ImageUrl = NormalizeOptional(request.ImageUrl);
        book.TotalStock = request.TotalStock;
        book.AvailableStock = request.AvailableStock;
        book.AuthorId = request.AuthorId;
        book.CategoryId = request.CategoryId;
        book.ShelfId = request.ShelfId;

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _dbContext.Books.FindAsync(id);

        if (book is null)
        {
            return NotFound(new
            {
                message = "Kitap bulunamadı."
            });
        }

        var hasReservations =
            await _dbContext.BookReservations.AnyAsync(
                reservation => reservation.BookId == id);

        if (hasReservations)
        {
            return Conflict(new
            {
                message =
                    "Rezervasyon kaydı bulunan kitap silinemez."
            });
        }

        _dbContext.Books.Remove(book);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    private async Task<BookDetailResponse?> GetBookDetailAsync(
        int id)
    {
        return await _dbContext.Books
            .AsNoTracking()
            .Where(book => book.Id == id)
            .Select(book => new BookDetailResponse
            {
                Id = book.Id,
                Title = book.Title,
                ISBN = book.ISBN,
                Description = book.Description,
                PublicationYear = book.PublicationYear,
                Publisher = book.Publisher,
                ImageUrl = book.ImageUrl,
                TotalStock = book.TotalStock,
                AvailableStock = book.AvailableStock,
                IsAvailable = book.AvailableStock > 0,
                AuthorId = book.AuthorId,
                AuthorName = book.Author.Name,
                CategoryId = book.CategoryId,
                CategoryName = book.Category.Name,
                ShelfId = book.ShelfId,
                ShelfCode = book.Shelf.Code,
                Floor = book.Shelf.Floor,
                Section = book.Shelf.Section,
                ShelfLocation =
                    $"{book.Shelf.Floor}. Kat - " +
                    $"{book.Shelf.Section} - " +
                    $"{book.Shelf.Code}"
            })
            .FirstOrDefaultAsync();
    }

    private async Task<string?> ValidateRelationsAsync(
        int authorId,
        int categoryId,
        int shelfId)
    {
        if (!await _dbContext.Authors.AnyAsync(
                author => author.Id == authorId))
        {
            return "Seçilen yazar bulunamadı.";
        }

        if (!await _dbContext.Categories.AnyAsync(
                category => category.Id == categoryId))
        {
            return "Seçilen kategori bulunamadı.";
        }

        if (!await _dbContext.Shelves.AnyAsync(
                shelf => shelf.Id == shelfId))
        {
            return "Seçilen raf bulunamadı.";
        }

        return null;
    }

    private static string NormalizeIsbn(string isbn)
    {
        return isbn
            .Trim()
            .Replace("-", string.Empty)
            .Replace(" ", string.Empty);
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}