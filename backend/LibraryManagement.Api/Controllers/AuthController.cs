using System.Security.Claims;
using LibraryManagement.Api.Constants;
using LibraryManagement.Api.DTOs.Auth;
using LibraryManagement.Api.Models;
using LibraryManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(
        RegisterRequest request)
    {
        var email = request.Email
            .Trim()
            .ToLowerInvariant();

        var studentNumber = request.StudentNumber
            .Trim()
            .ToUpperInvariant();

        var existingEmail =
            await _userManager.FindByEmailAsync(email);

        if (existingEmail is not null)
        {
            return Conflict(new
            {
                message =
                    "Bu e-posta adresiyle kayıtlı bir kullanıcı bulunuyor."
            });
        }

        var studentNumberExists =
            await _userManager.Users.AnyAsync(
                user => user.StudentNumber == studentNumber);

        if (studentNumberExists)
        {
            return Conflict(new
            {
                message =
                    "Bu öğrenci numarasıyla kayıtlı bir kullanıcı bulunuyor."
            });
        }

        var user = new ApplicationUser
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            StudentNumber = studentNumber,
            Email = email,
            UserName = email,
            EmailConfirmed = true
        };

        var createResult =
            await _userManager.CreateAsync(
                user,
                request.Password);

        if (!createResult.Succeeded)
        {
            return BadRequest(new
            {
                message = "Kullanıcı kaydı tamamlanamadı.",
                errors = createResult.Errors.Select(
                    error => error.Description)
            });
        }

        var roleResult = await _userManager.AddToRoleAsync(
            user,
            UserRoles.Student);

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message =
                        "Kullanıcı rolü eklenemedi."
                });
        }

        var response = await CreateAuthResponseAsync(user);

        return StatusCode(
            StatusCodes.Status201Created,
            response);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(
        LoginRequest request)
    {
        var email = request.Email
            .Trim()
            .ToLowerInvariant();

        var user = await _userManager.FindByEmailAsync(email);

        if (user is null ||
            !await _userManager.CheckPasswordAsync(
                user,
                request.Password))
        {
            return Unauthorized(new
            {
                message = "E-posta adresi veya parola hatalı."
            });
        }

        return Ok(await CreateAuthResponseAsync(user));
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<AuthUserResponse>> GetProfile()
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return NotFound(new
            {
                message = "Kullanıcı bulunamadı."
            });
        }

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new AuthUserResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty,
            StudentNumber = user.StudentNumber,
            Roles = roles.ToList()
        });
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpGet("admin-check")]
    public IActionResult CheckAdminAuthorization()
    {
        return Ok(new
        {
            message = "Admin yetkisi başarıyla doğrulandı."
        });
    }

    private async Task<AuthResponse> CreateAuthResponseAsync(
        ApplicationUser user)
    {
        var tokenResult =
            await _tokenService.CreateTokenAsync(user);

        var roles = await _userManager.GetRolesAsync(user);

        return new AuthResponse
        {
            Token = tokenResult.Token,
            ExpiresAt = tokenResult.ExpiresAt,
            User = new AuthUserResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                StudentNumber = user.StudentNumber,
                Roles = roles.ToList()
            }
        };
    }
}