namespace vangoplus_server.Application.DTOs
{
    public class AlertDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string Type { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
    }
}
