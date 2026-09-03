using Microsoft.AspNetCore.Mvc;
using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;
using vangoplus_server.Application.Requests;

namespace vangoplus_server.Controllers
{
    [ApiController]
    [Route("api/student-route-assignments")]
    public class StudentRouteAssignmentController : ControllerBase
    {
        private readonly IStudentRouteAssignmentHandler _handler;

        public StudentRouteAssignmentController(IStudentRouteAssignmentHandler handler)
        {
            _handler = handler;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _handler.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _handler.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StudentRouteAssignmentDto dto)
        {
            var result = await _handler.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] StudentRouteAssignmentDto dto)
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _handler.DeleteAsync(id);
            return NoContent();
        }
    }
}
