using vangoplus_server.Application.DTOs;

namespace vangoplus_server.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> CreateAsync(UserDto dto, string passwordPlain);
        Task<UserDto?> GetByIdAsync(int id);
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task UpdateAsync(int id, UserDto dto);
        Task DeleteAsync(int id);
        Task<UserDto?> GetByEmailAsync(string email);
        Task<UserDetailDto?> GetByIdWithSubscriptionsAsync(int id);
        // Login: returns UserDto on success, null on failure
        Task<UserDto?> LoginAsync(string email, string passwordPlain);
    }
}
