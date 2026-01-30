namespace ProjectManagement.API.Dtos;

public sealed class LoginRequestDto
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}

public sealed class LoginResponseDto
{
    public string Token { get; set; } = "";
    public string UserId { get; set; } = "";
    public string Role { get; set; } = "";
    public DateTime ExpiresAtUtc { get; set; }
}

public sealed class MeDto
{
    public string UserId { get; set; } = "";
    public string Username { get; set; } = "";
    public string Role { get; set; } = "";
}
