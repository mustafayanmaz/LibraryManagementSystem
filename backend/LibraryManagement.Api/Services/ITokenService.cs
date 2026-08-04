using LibraryManagement.Api.Models;

namespace LibraryManagement.Api.Services;

public interface ITokenService
{
    Task<TokenResult> CreateTokenAsync(ApplicationUser user);
}

public sealed record TokenResult(
    string Token,
    DateTime ExpiresAt
);