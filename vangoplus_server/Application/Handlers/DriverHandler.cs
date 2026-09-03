using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;

namespace vangoplus_server.Application.Handlers
{
    public class DriverHandler : IDriverHandler
    {
        private readonly IDriverService _service;

        public DriverHandler(IDriverService service)
        {
            _service = service;
        }

        public async Task<DriverDto> CreateAsync(DriverDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Name is required.", nameof(dto.Name));
            if (string.IsNullOrWhiteSpace(dto.LicenseNo))
                throw new ArgumentException("LicenseNo is required.", nameof(dto.LicenseNo));
            if (string.IsNullOrWhiteSpace(dto.Status))
                throw new ArgumentException("Status is required.", nameof(dto.Status));

            return await _service.CreateAsync(dto);
        }

        public async Task DeleteAsync(int id)
        {
            await _service.DeleteAsync(id);
        }

        public async Task<IEnumerable<DriverDto>> GetAllAsync()
        {
            return await _service.GetAllAsync();
        }

        public async Task<DriverDto?> GetByIdAsync(int id)
        {
            return await _service.GetByIdAsync(id);
        }

        public async Task UpdateAsync(int id, DriverDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Name is required.", nameof(dto.Name));
            if (string.IsNullOrWhiteSpace(dto.LicenseNo))
                throw new ArgumentException("LicenseNo is required.", nameof(dto.LicenseNo));
            if (string.IsNullOrWhiteSpace(dto.Status))
                throw new ArgumentException("Status is required.", nameof(dto.Status));

            await _service.UpdateAsync(id, dto);
        }
        public async Task UpdateStatusAsync(int driverId, string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("Status is required.", nameof(status));

            await _service.UpdateStatusAsync(driverId, status);
        }
    }
}
