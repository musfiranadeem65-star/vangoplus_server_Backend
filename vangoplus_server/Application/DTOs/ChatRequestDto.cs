namespace vangoplus_server.Application.DTOs
{
    public class ChatRequestDto
    {
        // There is deliberately no parent id here. Who is asking comes from
        // the signed-in token, never from the request body.
        public string Message { get; set; } = null!;
    }
}
