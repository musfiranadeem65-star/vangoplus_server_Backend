using vangoplus_server.Application.DTOs;

namespace vangoplus_server.Application.Interfaces
{
    public interface IRouteService
    {
        Task<RouteDto> CreateAsync(RouteDto dto);
        Task<IEnumerable<RouteDto>> GetAllAsync();
        Task<RouteDto?> GetByIdAsync(int id);
        Task UpdateAsync(int id, RouteDto dto);
        Task DeleteAsync(int id);
        Task UpdateStatusAsync(int routeId, string status);
    }
}
