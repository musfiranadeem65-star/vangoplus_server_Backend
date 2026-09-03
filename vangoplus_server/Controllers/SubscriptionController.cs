using Microsoft.AspNetCore.Mvc;
using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;

namespace vangoplus_server.Controllers
{
    [ApiController]
    [Route("api/subscriptions")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionHandler _handler;

        public SubscriptionController(ISubscriptionHandler handler)
        {
            _handler = handler;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _handler.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("users/{userId}/subscription")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var item = await _handler.GetByUserIdAsync(userId);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _handler.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SubscriptionDto dto)
        {
            var created = await _handler.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SubscriptionDto dto)
        {
            await _handler.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            await _handler.UpdateStatusAsync(id, request.Status);
            return NoContent();
        }

        public class UpdateStatusRequest
        {
            public string Status { get; set; } = null!;
        }
    }
}
