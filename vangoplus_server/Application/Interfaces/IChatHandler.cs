using vangoplus_server.Application.DTOs;

namespace vangoplus_server.Application.Interfaces
{
    public interface IChatHandler
    {
        Task<ChatResponseDto> AskAsync(ChatRequestDto dto);
    }
}
