using Microsoft.AspNetCore.Mvc;
using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;

namespace vangoplus_server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserHandler _handler;

        public UserController(IUserHandler handler)
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
        public async Task<IActionResult> Create([FromBody] UserCreateRequest request)
        {
            var dto = new UserDto { Name = request.Name, Email = request.Email, Phone = request.Phone, City = request.City, Role = request.Role, Status = request.Status };
            var created = await _handler.CreateAsync(dto, request.Password);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await _handler.LoginAsync(request);
            if (result == null)
                return Unauthorized(new { message = "Invalid email or password." });
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserDto dto)
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

    public class UserCreateRequest
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Phone { get; set; }
        public string? City { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }
    }
}
