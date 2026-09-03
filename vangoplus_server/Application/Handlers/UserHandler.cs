using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;

namespace vangoplus_server.Application.Handlers
{
    public class UserHandler : IUserHandler
    {
        private readonly IUserService _service;

public UserHandler(IUserService service)
{
    _service = service;
}

public async Task<UserDto> CreateAsync(UserDto dto, string passwordPlain)
{
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("Email is required.", nameof(dto.Email));
            if (string.IsNullOrWhiteSpace(passwordPlain) || passwordPlain.Length < 6)
                throw new ArgumentException("Password must be at least 6 characters.", nameof(passwordPlain));

// Ensure unique email
var existing = await _service.GetByEmailAsync(dto.Email);
if (existing != null)
    throw new InvalidOperationException("Email already in use.");

return await _service.CreateAsync(dto, passwordPlain);
        }

public async Task DeleteAsync(int id)
{
    await _service.DeleteAsync(id);
}

public async Task<IEnumerable<UserDto>> GetAllAsync()
{
    return await _service.GetAllAsync();
}

public async Task<UserDto?> GetByIdAsync(int id)
{
    var result = await _service.GetByIdWithSubscriptionsAsync(id);
    return (UserDto?)result;
}

public async Task UpdateAsync(int id, UserDto dto)
{
    if (string.IsNullOrWhiteSpace(dto.Email))
        throw new ArgumentException("Email is required.", nameof(dto.Email));
    await _service.UpdateAsync(id, dto);
}

public async Task<UserDetailDto?> GetByIdWithSubscriptionsAsync(int id)
{
    return await _service.GetByIdWithSubscriptionsAsync(id);
}

public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
{
    if (request == null) throw new ArgumentNullException(nameof(request));
    if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        throw new ArgumentException("Email and Password are required.");

    var user = await _service.LoginAsync(request.Email, request.Password);
    if (user == null) return null;

    return new LoginResponseDto
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        Role = user.Role,
        Phone = user.Phone,
        City = user.City,
        Status = user.Status
    };
}
    }
}
