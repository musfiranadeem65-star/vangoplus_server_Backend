namespace vangoplus_server.Application.DTOs
{
    public class SubscriptionPlanDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int MaxChildren { get; set; }
        public string Features { get; set; } = null!;
    }
}
