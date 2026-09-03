using Microsoft.AspNetCore.Mvc;
using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;
using RouteHandler = vangoplus_server.Application.Handlers.RouteHandler;
using IRouteHandler = vangoplus_server.Application.Interfaces.IRouteHandler;
using IRouteStopHandler = vangoplus_server.Application.Interfaces.IRouteStopHandler;
namespace vangoplus_server.Controllers
{
    [ApiController]
    [Route("api/routes")]
    public class RouteStopController : ControllerBase
    {
        private readonly IRouteStopHandler _handler;

        public RouteStopController(IRouteStopHandler handler)
        {
            _handler = handler;
        }

        [HttpGet("{routeId}/stops")]
        public async Task<IActionResult> GetByRoute(int routeId)
        {
            var list = await _handler.GetByRouteIdAsync(routeId);
            return Ok(list);
        }

        [HttpPost("{routeId}/stops")]
        public async Task<IActionResult> Create(int routeId, [FromBody] RouteStopDto dto)
        {
            dto.RouteId = routeId;
            var created = await _handler.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpGet("stops/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _handler.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPut("stops/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RouteStopDto dto)
        {
            await _handler.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("stops/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _handler.DeleteAsync(id);
            return NoContent();
        }
    }
}
