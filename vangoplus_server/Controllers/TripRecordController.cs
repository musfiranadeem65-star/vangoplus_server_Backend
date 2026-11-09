using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using vangoplus_server.Application.Auth;
using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;

namespace vangoplus_server.Controllers
{
    [ApiController]
    [Route("api/trip-records")]
    [Authorize]
    public class TripRecordController : ControllerBase
    {
        private readonly ITripRecordHandler _handler;

        public TripRecordController(ITripRecordHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// The roster for a route on one day, for the person marking children
        /// on and off the van.
        ///
        /// Staff only. It lists every child on the route by name, so a parent
        /// must not be able to open it: that would show them the other
        /// families travelling with their child.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("route/{routeId:int}")]
        public async Task<IActionResult> GetRouteDay(int routeId, [FromQuery] DateOnly? date)
        {
            try
            {
                return Ok(await _handler.GetRouteDayAsync(routeId, date));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// The signed-in parent's own children. The parent id comes from the
        /// token, so this cannot be pointed at another family.
        /// </summary>
        [HttpGet("my-children")]
        public async Task<IActionResult> GetMyChildren([FromQuery] int days = 7)
        {
            var parentUserId = User.UserId();
            if (parentUserId is null) return Unauthorized();

            return Ok(await _handler.GetForParentAsync(parentUserId.Value, days));
        }

        /// <summary>
        /// Records that a child boarded the van, or was dropped off.
        ///
        /// Staff only, for the same reason: a parent writing these could
        /// record events against another family's child.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Record([FromBody] TripRecordInputDto dto)
        {
            var userId = User.UserId();
            if (userId is null) return Unauthorized();

            try
            {
                var created = await _handler.RecordAsync(dto, userId.Value);
                return Ok(created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
