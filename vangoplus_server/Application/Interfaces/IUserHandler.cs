using vangoplus_server.Application.DTOs;

namespace vangoplus_server.Application.Interfaces
{
    public interface IUserHandler
    {
        Task<UserDto> CreateAsync(UserDto dto, string passwordPlain);
        Task<UserDto?> GetByIdAsync(int id);
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task UpdateAsync(int id, UserDto dto);
        Task DeleteAsync(int id);
        Task<UserDetailDto?> GetByIdWithSubscriptionsAsync(int id);
        // Login via handler - returns LoginResponseDto on success, null on failure
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
        // Change password (validated by handler)
        Task ChangePasswordAsync(ChangePasswordDto dto);
        // Get notification preferences for a user
        Task<NotificationPreferencesDto?> GetNotificationPreferencesAsync(int id);
        // Update notification preferences for a user
        Task UpdateNotificationPreferencesAsync(int id, NotificationPreferencesDto dto);
    }
}
