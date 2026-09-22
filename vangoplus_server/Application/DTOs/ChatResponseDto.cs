namespace vangoplus_server.Application.DTOs
{
    public class ChatResponseDto
    {
        public string Intent { get; set; } = null!;
        public float Confidence { get; set; }
        public string Answer { get; set; } = null!;
        public bool Escalated { get; set; }
    }
}
