using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;

namespace vangoplus_server.Application.Handlers
{
    public class SubscriptionPlanHandler : ISubscriptionPlanHandler
    {
        private readonly ISubscriptionPlanService _service;

        public SubscriptionPlanHandler(ISubscriptionPlanService service)
        {
            _service = service;
        }

        public async Task<SubscriptionPlanDto> CreateAsync(SubscriptionPlanDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Name is required.", nameof(dto.Name));
            if (dto.Price < 0)
                throw new ArgumentException("Price must be zero or greater.", nameof(dto.Price));
            if (dto.MaxChildren <= 0)
                throw new ArgumentException("MaxChildren must be greater than zero.", nameof(dto.MaxChildren));
            if (string.IsNullOrWhiteSpace(dto.Features))
                throw new ArgumentException("Features is required.", nameof(dto.Features));

            return await _service.CreateAsync(dto);
        }

        public async Task DeleteAsync(int id)
        {
            await _service.DeleteAsync(id);
        }

        public async Task<IEnumerable<SubscriptionPlanDto>> GetAllAsync()
        {
            return await _service.GetAllAsync();
        }

        public async Task<SubscriptionPlanDto?> GetByIdAsync(int id)
        {
            return await _service.GetByIdAsync(id);
        }

        public async Task UpdateAsync(int id, SubscriptionPlanDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Name is required.", nameof(dto.Name));
   
            if (dto.MaxChildren <= 0)
                throw new ArgumentException("MaxChildren must be greater than zero.", nameof(dto.MaxChildren));
            if (string.IsNullOrWhiteSpace(dto.Features))
                throw new ArgumentException("Features is required.", nameof(dto.Features));

            await _service.UpdateAsync(id, dto);
        }
    }
}
