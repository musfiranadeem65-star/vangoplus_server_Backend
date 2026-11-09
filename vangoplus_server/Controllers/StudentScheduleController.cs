using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using vangoplus_server.Application.Auth;
using vangoplus_server.Application.Interfaces;

namespace vangoplus_server.Controllers
{
    [ApiController]
    [Route("api/students")]
    [Authorize]
    public class StudentScheduleController : ControllerBase
    {
        private readonly IStudentScheduleService _schedules;

        public StudentScheduleController(IStudentScheduleService schedules)
        {
            _schedules = schedules;
        }

        /// <summary>
        /// One child's route, driver and stops.
        ///
        /// A parent may only read their own child. Without that check this
        /// would hand any signed-in parent another family's route and stop
        /// times by walking the student ids.
        /// </summary>
        [HttpGet("{studentId:int}/schedule")]
        public async Task<IActionResult> GetSchedule(int studentId)
        {
            var userId = User.UserId();
            if (userId is null) return Unauthorized();

            if (!User.IsAdmin() &&
                !await _schedules.BelongsToParentAsync(studentId, userId.Value))
                return Forbid();

            var schedule = await _schedules.GetAsync(studentId);
            if (schedule == null) return NotFound();

            return Ok(schedule);
        }
    }
}
