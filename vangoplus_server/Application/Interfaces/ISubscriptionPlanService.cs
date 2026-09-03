using vangoplus_server.Application.DTOs;

namespace vangoplus_server.Application.Interfaces
{
    public interface ISubscriptionPlanService
    {
        Task<SubscriptionPlanDto> CreateAsync(SubscriptionPlanDto dto);
        Task<IEnumerable<SubscriptionPlanDto>> GetAllAsync();
        Task<SubscriptionPlanDto?> GetByIdAsync(int id);
        Task UpdateAsync(int id, SubscriptionPlanDto dto);
        Task DeleteAsync(int id);
    }
}
