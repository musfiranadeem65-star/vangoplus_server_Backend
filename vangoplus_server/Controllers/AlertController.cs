using Microsoft.AspNetCore.Mvc;
using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;

namespace vangoplus_server.Controllers
{
    [ApiController]
    [Route("api")]
    public class AlertController : ControllerBase
    {
        private readonly IAlertHandler _handler;

        public AlertController(IAlertHandler handler)
        {
            _handler = handler;
        }

        [HttpGet("students/{studentId}/alerts")]
        public async Task<IActionResult> GetByStudentId(int studentId)
        {
            var alerts = await _handler.GetByStudentIdAsync(studentId);
            return Ok(alerts);
        }

        [HttpGet("alerts/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var alert = await _handler.GetByIdAsync(id);
            if (alert == null) return NotFound();
            return Ok(alert);
        }

        [HttpPost("alerts")]
        public async Task<IActionResult> Create([FromBody] AlertDto dto)
        {
            var created = await _handler.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPatch("alerts/{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id, [FromBody] MarkAsReadRequest request)
        {
            if (!request.IsRead)
                return BadRequest("IsRead must be true.");

            await _handler.UpdateReadStatusAsync(id);
            return NoContent();
        }

        [HttpGet("students/{studentId}/schedule")]
        public async Task<IActionResult> GetSchedule(int studentId)
        {
            // This endpoint is for future implementation
            // Returns schedule information for a student
            return Ok(new { message = "Schedule endpoint for student " + studentId });
        }

        public class MarkAsReadRequest
        {
            public bool IsRead { get; set; }
        }
    }
}
