namespace vangoplus_server.Domain.Entities
{
    public class SubscriptionPlan
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int MaxChildren { get; set; }
        public string Features { get; set; } = null!;

        // Navigation
        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }
}
