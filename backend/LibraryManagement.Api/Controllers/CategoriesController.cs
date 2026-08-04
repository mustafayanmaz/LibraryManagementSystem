using LibraryManagement.Api.Constants;
using LibraryManagement.Api.Data;
using LibraryManagement.Api.DTOs.Categories;
using LibraryManagement.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public CategoriesController(
        ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<
        IReadOnlyList<CategoryResponse>>> GetAll()
    {
        var categories = await _dbContext.Categories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .Select(category => new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                BookCount = category.Books.Count
            })
            .ToListAsync();

        return Ok(categories);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryResponse>> GetById(
        int id)
    {
        var category = await _dbContext.Categories
            .AsNoTracking()
            .Where(category => category.Id == id)
            .Select(category => new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                BookCount = category.Books.Count
            })
            .FirstOrDefaultAsync();

        if (category is null)
        {
            return NotFound(new
            {
                message = "Kategori bulunamadı."
            });
        }

        return Ok(category);
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> Create(
        CategoryUpsertRequest request)
    {
        var name = request.Name.Trim();

        var exists = await _dbContext.Categories.AnyAsync(
            category =>
                EF.Functions.ILike(category.Name, name));

        if (exists)
        {
            return Conflict(new
            {
                message = "Bu kategori zaten bulunuyor."
            });
        }

        var category = new Category
        {
            Name = name,
            Description = NormalizeOptional(
                request.Description)
        };

        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();

        var response = new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            BookCount = 0
        };

        return CreatedAtAction(
            nameof(GetById),
            new { id = category.Id },
            response);
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        CategoryUpsertRequest request)
    {
        var category = await _dbContext.Categories.FindAsync(id);

        if (category is null)
        {
            return NotFound(new
            {
                message = "Kategori bulunamadı."
            });
        }

        var name = request.Name.Trim();

        var exists = await _dbContext.Categories.AnyAsync(
            existingCategory =>
                existingCategory.Id != id &&
                EF.Functions.ILike(
                    existingCategory.Name,
                    name));

        if (exists)
        {
            return Conflict(new
            {
                message =
                    "Bu isimde başka bir kategori bulunuyor."
            });
        }

        category.Name = name;
        category.Description = NormalizeOptional(
            request.Description);

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _dbContext.Categories.FindAsync(id);

        if (category is null)
        {
            return NotFound(new
            {
                message = "Kategori bulunamadı."
            });
        }

        var isUsed = await _dbContext.Books.AnyAsync(
            book => book.CategoryId == id);

        if (isUsed)
        {
            return Conflict(new
            {
                message =
                    "Kitap kaydı bulunan kategori silinemez."
            });
        }

        _dbContext.Categories.Remove(category);
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