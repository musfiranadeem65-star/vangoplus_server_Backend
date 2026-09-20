using Microsoft.AspNetCore.Mvc;
using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;

namespace vangoplus_server.Controllers
{
    [ApiController]
    [Route("api")]
    public class ChatController : ControllerBase
    {
        private readonly IChatHandler _handler;

        public ChatController(IChatHandler handler) => _handler = handler;

        [HttpPost("chat")]
        public async Task<IActionResult> Ask([FromBody] ChatRequestDto dto)
        {
            try
            {
                var response = await _handler.AskAsync(dto);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
