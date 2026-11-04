using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using vangoplus_server.Application.Auth;
using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;

namespace vangoplus_server.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatHandler _handler;

        public ChatController(IChatHandler handler) => _handler = handler;

        [HttpPost("chat")]
        public async Task<IActionResult> Ask([FromBody] ChatRequestDto dto)
        {
            // Who is asking comes from the token, not from the body.
            var parentUserId = User.UserId();
            if (parentUserId is null) return Unauthorized();

            try
            {
                var response = await _handler.AskAsync(parentUserId.Value, dto);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
