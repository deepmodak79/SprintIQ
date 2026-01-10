using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SprintIQ.API.Data;
using SprintIQ.API.DTOs;
using SprintIQ.API.Models;

namespace SprintIQ.API.Services;

public class AuthService : IAuthService
{
    private readonly SprintIQDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(SprintIQDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
        {
            return null;
        }

        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = HashPassword(dto.Password),
            Role = "Member",
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(user.Id, user.Email, user.Role ?? "Member");
        var expiry = DateTime.UtcNow.AddDays(7);

        return new AuthResponseDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role ?? "Member",
            Token = token,
            ExpiresAt = expiry
        };
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        
        if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
        {
            return null;
        }

        user.LastActiveAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(user.Id, user.Email, user.Role ?? "Member");
        var expiry = DateTime.UtcNow.AddDays(7);

        return new AuthResponseDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role ?? "Member",
            Token = token,
            ExpiresAt = expiry
        };
    }

    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Badges)
            .ThenInclude(ub => ub.Badge)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return null;

        return MapToUserDto(user);
    }

    public async Task<UserDto?> UpdateUserAsync(int userId, string? fullName, string? avatarUrl)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return null;

        if (fullName != null) user.FullName = fullName;
        if (avatarUrl != null) user.AvatarUrl = avatarUrl;

        await _context.SaveChangesAsync();

        return await GetUserByIdAsync(userId);
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _context.Users
            .Include(u => u.Badges)
            .ThenInclude(ub => ub.Badge)
            .Where(u => u.IsActive)
            .ToListAsync();

        return users.Select(MapToUserDto).ToList();
    }

    public string GenerateJwtToken(int userId, string email, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Key"] ?? "SprintIQSuperSecretKeyForJWTAuthentication2024!"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "SprintIQ",
            audience: _configuration["Jwt:Audience"] ?? "SprintIQ",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private static bool VerifyPassword(string password, string hash)
    {
        return HashPassword(password) == hash;
    }

    private static UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            AvatarUrl = user.AvatarUrl,
            TotalPoints = user.TotalPoints,
            CurrentStreak = user.CurrentStreak,
            LongestStreak = user.LongestStreak,
            LastActiveAt = user.LastActiveAt,
            Badges = user.Badges.Select(ub => new BadgeDto
            {
                Id = ub.Badge.Id,
                Name = ub.Badge.Name,
                Description = ub.Badge.Description,
                Icon = ub.Badge.Icon,
                Color = ub.Badge.Color,
                EarnedAt = ub.EarnedAt,
                Count = ub.Count
            }).ToList()
        };
    }
}
