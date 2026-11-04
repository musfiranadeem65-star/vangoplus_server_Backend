using vangoplus_server.Application.DTOs;

namespace vangoplus_server.Application.Interfaces
{
    public interface IChatHandler
    {
        /// <summary>
        /// Answers one question for one parent. The parent id is a separate
        /// argument, and the controller takes it from the signed-in token, so
        /// a caller cannot ask about somebody else's children by putting a
        /// different id in the request body.
        /// </summary>
        Task<ChatResponseDto> AskAsync(int parentUserId, ChatRequestDto dto);
    }
}
