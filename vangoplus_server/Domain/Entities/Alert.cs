namespace vangoplus_server.Domain.Entities
{
    public class Alert
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string Type { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; } = false;

        // Navigation
        public Student Student { get; set; } = null!;
    }
}
