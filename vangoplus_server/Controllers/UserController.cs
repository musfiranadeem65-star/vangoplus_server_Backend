using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using vangoplus_server.Application.Auth;
using Microsoft.AspNetCore.Mvc;
using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;

namespace vangoplus_server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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

        // The public signup form. Anyone may create a parent account; the
        // role is forced below so a caller cannot register themselves as an
        // administrator.
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreateRequest request)
        {
            // Self-registration always produces a Parent. Staff accounts are
            // created by an administrator, not by this endpoint.
            var role = User.IsAdmin() ? request.Role : "Parent";

            var dto = new UserDto { Name = request.Name, Email = request.Email, Phone = request.Phone, City = request.City, Role = role, Status = request.Status };
            var created = await _handler.CreateAsync(dto, request.Password);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await _handler.LoginAsync(request);
            if (result == null)
                return Unauthorized(new { message = "Invalid email or password." });
            return Ok(result);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            if (request is null)
                return BadRequest(new { message = "A request body is required." });

            // Being signed in is not enough: it has to be your own account.
            // Without this, any parent could change any other parent's
            // password as long as they knew the email address.
            if (!User.IsAdmin())
            {
                var signedInEmail = User.FindFirstValue(ClaimTypes.Email);

                if (signedInEmail is null ||
                    !string.Equals(signedInEmail, request.Email, StringComparison.OrdinalIgnoreCase))
                    return Forbid();
            }

            try
            {
                await _handler.ChangePasswordAsync(request);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

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

        [HttpGet("{id}/notification-preferences")]
        public async Task<IActionResult> GetNotificationPreferences(int id)
        {
            var prefs = await _handler.GetNotificationPreferencesAsync(id);
            if (prefs == null)
                return NotFound(new { message = "User not found." });
            return Ok(prefs);
        }

        [HttpPut("{id}/notification-preferences")]
        public async Task<IActionResult> UpdateNotificationPreferences(int id, [FromBody] NotificationPreferencesDto dto)
        {
            try
            {
                await _handler.UpdateNotificationPreferencesAsync(id, dto);
                var updated = await _handler.GetNotificationPreferencesAsync(id);
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
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
