using vangoplus_server.Application.DTOs;

namespace vangoplus_server.Application.Interfaces
{
    public interface IRouteStopHandler
    {
        Task<RouteStopDto> CreateAsync(RouteStopDto dto);
        Task<IEnumerable<RouteStopDto>> GetByRouteIdAsync(int routeId);
        Task<RouteStopDto?> GetByIdAsync(int id);
        Task UpdateAsync(int id, RouteStopDto dto);
        Task DeleteAsync(int id);
    }
}
