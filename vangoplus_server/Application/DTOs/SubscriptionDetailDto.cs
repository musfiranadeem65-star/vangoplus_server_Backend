namespace vangoplus_server.Application.DTOs
{
    public class SubscriptionDetailDto : SubscriptionDto
    {
        // Navigation - include full user details (without nested subscriptions to avoid circular reference)
        public UserDto? User { get; set; }
    }
}
