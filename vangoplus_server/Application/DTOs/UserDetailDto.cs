namespace vangoplus_server.Application.DTOs
{
    public class UserDetailDto : UserDto
    {
        public DateTime CreatedAt { get; set; }

        // Navigation - include all subscriptions for this user
        public ICollection<SubscriptionDto> Subscriptions { get; set; } = new List<SubscriptionDto>();
    }
}
