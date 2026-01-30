using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProjectManagement.API.Dtos;
using ProjectManagement.BLL.Interface;
using ProjectManagement.BLL.Interfaces;
using ProjectManagement.Common.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectManagement.BLL.Service;

public sealed class AuthService : IAuthService
{
    private readonly IConfiguration _config;

    // Demo users (replace later with DB lookup)
    private static readonly List<(string UserId, string Username, string Password, string Role)> Users =
        new()
        {
            ("A001", "admin",   "admin123",   "Admin"),
            ("M001", "manager", "manager123", "Manager"),
            ("U001", "user",    "user123",    "User")
        };

    public AuthService(IConfiguration config)
    {
        _config = config;
    }

    public LoginResponseDto Login(LoginRequestDto req)
    {
        if (req == null) throw new UnauthorizedAccessException("Invalid login request");

        var u = Users.FirstOrDefault(x =>
            string.Equals(x.Username, req.Username?.Trim(), StringComparison.OrdinalIgnoreCase) &&
            x.Password == (req.Password ?? ""));

        if (string.IsNullOrWhiteSpace(u.UserId))
            throw new UnauthorizedAccessException("Invalid username or password");

        var expiryMinutes = 60;
        var expiryText = _config["Jwt:ExpiryMinutes"];
        if (!string.IsNullOrWhiteSpace(expiryText) && int.TryParse(expiryText, out var m) && m > 0)
            expiryMinutes = m;

        var expiresAtUtc = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var token = CreateToken(u.UserId, u.Username, u.Role, expiresAtUtc);

        return new LoginResponseDto
        {
            Token = token,
            UserId = u.UserId,
            Role = u.Role,
            ExpiresAtUtc = expiresAtUtc
        };
    }

    public MeDto Me(ClaimsPrincipal user)
    {
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        var username = user?.FindFirst(ClaimTypes.Name)?.Value ?? "";
        var role = user?.FindFirst(ClaimTypes.Role)?.Value ?? "";

        return new MeDto
        {
            UserId = userId,
            Username = username,
            Role = role
        };
    }

    private string CreateToken(string userId, string username, string role, DateTime expiresAtUtc)
    {
        var key = _config["Jwt:Key"];
        var issuer = _config["Jwt:Issuer"];
        var audience = _config["Jwt:Audience"];

        if (string.IsNullOrWhiteSpace(key)) throw new Exception("Missing Jwt:Key in appsettings.json");
        if (string.IsNullOrWhiteSpace(issuer)) throw new Exception("Missing Jwt:Issuer in appsettings.json");
        if (string.IsNullOrWhiteSpace(audience)) throw new Exception("Missing Jwt:Audience in appsettings.json");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}
