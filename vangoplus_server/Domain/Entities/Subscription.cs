namespace vangoplus_server.Domain.Entities
{
    public class Subscription
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PlanId { get; set; }
        public string PlanName { get; set; } = null!;
        public decimal Price { get; set; }
        public string Status { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
        public DateTime StartedAt { get; set; }

        // Navigation
        public User User { get; set; } = null!;
        public SubscriptionPlan SubscriptionPlan { get; set; } = null!;
    }
}
