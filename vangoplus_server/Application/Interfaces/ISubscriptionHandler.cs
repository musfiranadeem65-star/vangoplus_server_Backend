using vangoplus_server.Application.DTOs;

namespace vangoplus_server.Application.Interfaces
{
    public interface ISubscriptionHandler
    {
        Task<SubscriptionDto> CreateAsync(SubscriptionDto dto);
        Task<IEnumerable<SubscriptionDto>> GetAllAsync();
        Task<SubscriptionDto?> GetByUserIdAsync(int userId);
        Task<SubscriptionDto?> GetByIdAsync(int id);
        Task UpdateAsync(int id, SubscriptionDto dto);
        Task UpdateStatusAsync(int subscriptionId, string status);
        Task<SubscriptionDetailDto?> GetByIdWithUserAsync(int id);
    }
}
