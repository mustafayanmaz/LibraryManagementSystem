namespace LibraryManagement.Api.DTOs.Auth;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public AuthUserResponse User { get; set; } = new();
}

public class AuthUserResponse
{
    public string Id { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? StudentNumber { get; set; }

    public List<string> Roles { get; set; } = new();
}