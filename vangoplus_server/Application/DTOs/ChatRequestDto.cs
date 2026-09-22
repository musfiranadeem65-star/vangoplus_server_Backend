namespace vangoplus_server.Application.DTOs
{
    public class ChatRequestDto
    {
        public int ParentUserId { get; set; }
        public string Message { get; set; } = null!;
    }
}
