using Microsoft.AspNetCore.Mvc;
using vangoplus_server.Application.DTOs;
namespace vangoplus_server.Controllers;
using RouteHandler = vangoplus_server.Application.Handlers.RouteHandler;
using IRouteHandler = vangoplus_server.Application.Interfaces.IRouteHandler;

[ApiController]
[Route("api/[controller]")]
public class RouteController : ControllerBase
{
    private readonly IRouteHandler _handler;

    public RouteController(IRouteHandler handler)
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
    public async Task<IActionResult> Create([FromBody] RouteDto dto)
    {
        var created = await _handler.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] RouteDto dto)
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

    public class UpdateStatusRequest
    {
        public string Status { get; set; } = null!;
    }
}
