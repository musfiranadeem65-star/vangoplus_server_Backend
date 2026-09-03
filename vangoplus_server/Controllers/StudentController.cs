using Microsoft.AspNetCore.Mvc;
using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;

namespace vangoplus_server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentHandler _handler;

        public StudentController(IStudentHandler handler)
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

        [HttpGet("parent/{parentId}")]
        public async Task<IActionResult> GetByParent(int parentId)
        {
            var list = await _handler.GetByParentIdAsync(parentId);
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StudentCreateRequest request)
        {
            var dto = new StudentDto { ParentUserId = request.ParentUserId, Name = request.Name, Grade = request.Grade, Section = request.Section, Status = request.Status };
            var created = await _handler.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] StudentDto dto)
        {
            await _handler.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _handler.DeleteAsync(id);
            return NoContent();
        }
    }

    public class StudentCreateRequest
    {
        public int ParentUserId { get; set; }
        public string Name { get; set; } = null!;
        public string Grade { get; set; } = null!;
        public string? Section { get; set; }
        public string? Status { get; set; }
    }
}
