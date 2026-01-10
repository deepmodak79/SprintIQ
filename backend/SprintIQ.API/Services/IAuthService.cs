using SprintIQ.API.DTOs;

namespace SprintIQ.API.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    Task<UserDto?> GetUserByIdAsync(int userId);
    Task<UserDto?> UpdateUserAsync(int userId, string? fullName, string? avatarUrl);
    Task<List<UserDto>> GetAllUsersAsync();
    string GenerateJwtToken(int userId, string email, string role);
}
