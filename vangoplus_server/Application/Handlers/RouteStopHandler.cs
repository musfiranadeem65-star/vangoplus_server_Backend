using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;
using Route = vangoplus_server.Domain.Entities.RouteStop;
namespace vangoplus_server.Application.Handlers
{
    public class RouteStopHandler : IRouteStopHandler
    {
        private readonly IRouteStopService _service;
        private readonly IRouteService _routeService;

        public RouteStopHandler(IRouteStopService service, IRouteService routeService)
        {
            _service = service;
            _routeService = routeService;
        }

        public async Task<RouteStopDto> CreateAsync(RouteStopDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.StopName))
                throw new ArgumentException("StopName is required.", nameof(dto.StopName));
            if (dto.RouteId <= 0)
                throw new ArgumentException("RouteId is required.", nameof(dto.RouteId));
            if (dto.OrderIndex < 0)
                throw new ArgumentException("OrderIndex must be greater than or equal to zero.", nameof(dto.OrderIndex));

            // Verify route exists
            var route = await _routeService.GetByIdAsync(dto.RouteId);
            if (route == null)
                throw new InvalidOperationException("Route not found.");

            return await _service.CreateAsync(dto);
        }

        public async Task DeleteAsync(int id)
        {
            await _service.DeleteAsync(id);
        }

        public async Task<RouteStopDto?> GetByIdAsync(int id)
        {
            return await _service.GetByIdAsync(id);
        }

        public async Task<IEnumerable<RouteStopDto>> GetByRouteIdAsync(int routeId)
        {
            return await _service.GetByRouteIdAsync(routeId);
        }

        public async Task UpdateAsync(int id, RouteStopDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.StopName))
                throw new ArgumentException("StopName is required.", nameof(dto.StopName));
            if (dto.RouteId <= 0)
                throw new ArgumentException("RouteId is required.", nameof(dto.RouteId));
            if (dto.OrderIndex < 0)
                throw new ArgumentException("OrderIndex must be greater than or equal to zero.", nameof(dto.OrderIndex));

            // Verify route exists
            var route = await _routeService.GetByIdAsync(dto.RouteId);
            if (route == null)
                throw new InvalidOperationException("Route not found.");

            await _service.UpdateAsync(id, dto);
        }
    }
}
