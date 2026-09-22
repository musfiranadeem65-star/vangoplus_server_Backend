using Microsoft.AspNetCore.Mvc;
using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;

namespace vangoplus_server.Controllers
{
    [ApiController]
    [Route("api/school-settings")]
    public class SchoolSettingsController : ControllerBase
    {
        private readonly ISchoolSettingHandler _handler;

        public SchoolSettingsController(ISchoolSettingHandler handler)
        {
            _handler = handler;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var dto = await _handler.GetAsync();
            return Ok(dto);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateSchoolSettingDto dto)
        {
            try
            {
                var updated = await _handler.UpdateAsync(dto);
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
