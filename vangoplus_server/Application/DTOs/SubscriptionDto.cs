namespace vangoplus_server.Application.DTOs
{
    public class SubscriptionDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int PlanId { get; set; }

        public string PlanName { get; set; } = null!;

        public decimal Price { get; set; }

        public string Status { get; set; } = null!;

        public string PaymentMethod { get; set; } = null!;

        public DateTime StartedAt { get; set; }

        // JazzCash payment information
        // Nullable so existing API requests continue to work.
        public string? JazzCashNumber { get; set; }

        public string? TransactionId { get; set; }

        public string? PaymentStatus { get; set; }

        public DateTime? PaidAt { get; set; }

        // Extended for Admin UI
        public string? ParentName { get; set; }

        public string? StudentName { get; set; }
    }
}