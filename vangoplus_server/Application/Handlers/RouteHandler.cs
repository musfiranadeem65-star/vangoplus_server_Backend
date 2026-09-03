using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;
using IRouteHandler = vangoplus_server.Application.Interfaces.IRouteHandler;
namespace vangoplus_server.Application.Handlers
{
    public class RouteHandler : IRouteHandler
    {
        private readonly IRouteService _service;
        private readonly IDriverService _driverService;

        public RouteHandler(IRouteService service, IDriverService driverService)
        {
            _service = service;
            _driverService = driverService;
        }

        public async Task<RouteDto> CreateAsync(RouteDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Name is required.", nameof(dto.Name));
            if (string.IsNullOrWhiteSpace(dto.Status))
                throw new ArgumentException("Status is required.", nameof(dto.Status));
            if (dto.DriverId <= 0)
                throw new ArgumentException("DriverId is required.", nameof(dto.DriverId));

            // Verify driver exists
            var driver = await _driverService.GetByIdAsync(dto.DriverId);
            if (driver == null)
                throw new InvalidOperationException("Driver not found.");

            // Validate route stops if provided
            if (dto.RouteStops != null && dto.RouteStops.Count > 0)
            {
                foreach (var stop in dto.RouteStops)
                {
                    if (string.IsNullOrWhiteSpace(stop.StopName))
                        throw new ArgumentException("Route stop name is required.", nameof(stop.StopName));
                    if (stop.OrderIndex < 0)
                        throw new ArgumentException("Route stop OrderIndex cannot be negative.", nameof(stop.OrderIndex));
                }
            }

            return await _service.CreateAsync(dto);
        }

        public async Task DeleteAsync(int id)
        {
            await _service.DeleteAsync(id);
        }

        public async Task<IEnumerable<RouteDto>> GetAllAsync()
        {
            return await _service.GetAllAsync();
        }

        public async Task<RouteDto?> GetByIdAsync(int id)
        {
            return await _service.GetByIdAsync(id);
        }

        public async Task UpdateAsync(int id, RouteDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Name is required.", nameof(dto.Name));
            if (string.IsNullOrWhiteSpace(dto.Status))
                throw new ArgumentException("Status is required.", nameof(dto.Status));
            if (dto.DriverId <= 0)
                throw new ArgumentException("DriverId is required.", nameof(dto.DriverId));

            // Verify driver exists
            var driver = await _driverService.GetByIdAsync(dto.DriverId);
            if (driver == null)
                throw new InvalidOperationException("Driver not found.");

            // Validate route stops if provided
            if (dto.RouteStops != null && dto.RouteStops.Count > 0)
            {
                foreach (var stop in dto.RouteStops)
                {
                    if (string.IsNullOrWhiteSpace(stop.StopName))
                        throw new ArgumentException("Route stop name is required.", nameof(stop.StopName));
                    if (stop.OrderIndex < 0)
                        throw new ArgumentException("Route stop OrderIndex cannot be negative.", nameof(stop.OrderIndex));
                }
            }

            await _service.UpdateAsync(id, dto);
        }

        public async Task UpdateStatusAsync(int routeId, string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("Status is required.", nameof(status));

            var route = await _service.GetByIdAsync(routeId);
            if (route == null)
                throw new InvalidOperationException("Route not found.");

            await _service.UpdateStatusAsync(routeId, status);
        }
    }
}
