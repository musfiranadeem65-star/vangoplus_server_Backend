using vangoplus_server.Application.DTOs;

namespace vangoplus_server.Application.Interfaces
{
    public interface IDriverHandler
    {
        Task<DriverDto> CreateAsync(DriverDto dto);
        Task<IEnumerable<DriverDto>> GetAllAsync();
        Task<DriverDto?> GetByIdAsync(int id);
        Task UpdateAsync(int id, DriverDto dto);
        Task UpdateStatusAsync(int driverId, string status);
        Task DeleteAsync(int id);
    }
}
