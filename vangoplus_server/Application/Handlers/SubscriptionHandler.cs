using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;

namespace vangoplus_server.Application.Handlers
{
    public class SubscriptionHandler : ISubscriptionHandler
    {
        private readonly ISubscriptionService _service;
        private readonly IUserService _userService;
        private readonly ISubscriptionPlanService _subscriptionPlanService;

        public SubscriptionHandler(
            ISubscriptionService service,
            IUserService userService,
            ISubscriptionPlanService subscriptionPlanService)
        {
            _service = service;
            _userService = userService;
            _subscriptionPlanService = subscriptionPlanService;
        }

        public async Task<SubscriptionDto> CreateAsync(SubscriptionDto dto)
        {
            if (dto.UserId <= 0)
                throw new ArgumentException("UserId must be greater than zero.", nameof(dto.UserId));
            if (dto.PlanId <= 0)
                throw new ArgumentException("PlanId must be greater than zero.", nameof(dto.PlanId));
            if (string.IsNullOrWhiteSpace(dto.Status))
                throw new ArgumentException("Status is required.", nameof(dto.Status));

            // Check if User exists
            var user = await _userService.GetByIdAsync(dto.UserId);
            if (user == null)
                throw new InvalidOperationException("User not found.");

            // Check if SubscriptionPlan exists
            var plan = await _subscriptionPlanService.GetByIdAsync(dto.PlanId);
            if (plan == null)
                throw new InvalidOperationException("Subscription Plan not found.");

            return await _service.CreateAsync(dto);
        }

        public async Task<IEnumerable<SubscriptionDto>> GetAllAsync()
        {
            return await _service.GetAllAsync();
        }

        public async Task<SubscriptionDto?> GetByUserIdAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("UserId must be greater than zero.", nameof(userId));

            return await _service.GetByUserIdAsync(userId);
        }

        public async Task<SubscriptionDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than zero.", nameof(id));

            var result = await _service.GetByIdWithUserAsync(id);
            return (SubscriptionDto?)result;
        }

        public async Task UpdateAsync(int id, SubscriptionDto dto)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than zero.", nameof(id));
            if (dto.UserId <= 0)
                throw new ArgumentException("UserId must be greater than zero.", nameof(dto.UserId));
            if (dto.PlanId <= 0)
                throw new ArgumentException("PlanId must be greater than zero.", nameof(dto.PlanId));
            if (string.IsNullOrWhiteSpace(dto.Status))
                throw new ArgumentException("Status is required.", nameof(dto.Status));

            // Check if User exists
            var user = await _userService.GetByIdAsync(dto.UserId);
            if (user == null)
                throw new InvalidOperationException("User not found.");

            // Check if SubscriptionPlan exists
            var plan = await _subscriptionPlanService.GetByIdAsync(dto.PlanId);
            if (plan == null)
                throw new InvalidOperationException("Subscription Plan not found.");

            await _service.UpdateAsync(id, dto);
        }

        public async Task UpdateStatusAsync(int subscriptionId, string status)
        {
            if (subscriptionId <= 0)
                throw new ArgumentException("SubscriptionId must be greater than zero.", nameof(subscriptionId));
            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("Status is required.", nameof(status));

            await _service.UpdateStatusAsync(subscriptionId, status);
        }

        public async Task<SubscriptionDetailDto?> GetByIdWithUserAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than zero.", nameof(id));

            return await _service.GetByIdWithUserAsync(id);
        }
    }
}
