using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace vangoplus_server.Controllers
{
    [ApiController]
    [Route("api/students/{studentId}/guardians")]
    public class GuardianController : ControllerBase
    {
        private readonly IGuardianHandler _handler;
        private readonly IWebHostEnvironment _env;

        public GuardianController(IGuardianHandler handler, IWebHostEnvironment env)
        {
            _handler = handler;
            _env = env;
        }

        [HttpGet("/api/Guardian")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _handler.GetAllAsync();
            return Ok(list);
        }

        [HttpPost("/api/Guardian")]
        public async Task<IActionResult> Create([FromForm] GuardianDto dto, IFormFile? identityDocument)
        {
            if (identityDocument != null)
            {
                // Validate size (<= 5 MB)
                const long MaxBytes = 5 * 1024 * 1024;
                if (identityDocument.Length > MaxBytes)
                    return BadRequest("File is too large. Maximum allowed size is 5 MB.");

                // Validate extension
                var ext = Path.GetExtension(identityDocument.FileName).ToLowerInvariant();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
                if (!allowed.Contains(ext))
                    return BadRequest("Unsupported file type. Allowed types: JPG, JPEG, PNG, PDF.");

                // Ensure uploads directory
                var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
                var uploadsDir = Path.Combine(webRoot, "uploads");
                Directory.CreateDirectory(uploadsDir);

                var fileName = Guid.NewGuid().ToString("N") + ext;
                var physicalPath = Path.Combine(uploadsDir, fileName);
                using (var stream = System.IO.File.Create(physicalPath))
                {
                    await identityDocument.CopyToAsync(stream);
                }

                dto.IdentityDocumentPath = "/uploads/" + fileName;
            }

            var created = await _handler.CreateAsync(dto);
            return CreatedAtAction(nameof(GetByStudent), new { studentId = 0 }, created);
        }

        [HttpGet]
        public async Task<IActionResult> GetByStudent(int studentId)
        {
            var list = await _handler.GetByStudentIdAsync(studentId);
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> LinkToStudent(int studentId, [FromBody] LinkGuardianRequest request)
        {
            var linkDto = new StudentGuardianDto 
            { 
                StudentId = studentId, 
                GuardianId = request.GuardianId, 
                IsPrimary = request.IsPrimary, 
                Status = request.Status 
            };
            await _handler.LinkToStudentAsync(studentId, request.GuardianId, linkDto);
            return Created();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int studentId, int id, [FromForm] GuardianDto dto, IFormFile? identityDocument)
        {
            // If a new file is provided, validate and save it; delete previous file if exists
            if (identityDocument != null)
            {
                const long MaxBytes = 5 * 1024 * 1024;
                if (identityDocument.Length > MaxBytes)
                    return BadRequest("File is too large. Maximum allowed size is 5 MB.");

                var ext = Path.GetExtension(identityDocument.FileName).ToLowerInvariant();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
                if (!allowed.Contains(ext))
                    return BadRequest("Unsupported file type. Allowed types: JPG, JPEG, PNG, PDF.");

                var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
                var uploadsDir = Path.Combine(webRoot, "uploads");
                Directory.CreateDirectory(uploadsDir);

                var fileName = Guid.NewGuid().ToString("N") + ext;
                var physicalPath = Path.Combine(uploadsDir, fileName);
                using (var stream = System.IO.File.Create(physicalPath))
                {
                    await identityDocument.CopyToAsync(stream);
                }

                // Delete previous file if exists and is inside /uploads
                var existing = await _handler.GetByIdAsync(id);
                if (existing?.IdentityDocumentPath != null)
                {
                    var prev = existing.IdentityDocumentPath;
                    if (prev.StartsWith("/uploads/"))
                    {
                        var prevName = Path.GetFileName(prev);
                        var prevPhysical = Path.Combine(uploadsDir, prevName);
                        if (System.IO.File.Exists(prevPhysical))
                        {
                            try { System.IO.File.Delete(prevPhysical); } catch { /* ignore delete errors */ }
                        }
                    }
                }

                dto.IdentityDocumentPath = "/uploads/" + fileName;
            }

            await _handler.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpPatch("/api/Guardian/{id}/status")]
        public async Task<IActionResult> UpdateStatus( int id, [FromBody] UpdateStatusRequest request)
        {
            await _handler.UpdateStatusAsync(id, request.Status);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int studentId, int id)
        {
            await _handler.DeleteAsync(id);
            return NoContent();
        }
    }

    public class LinkGuardianRequest
    {
        public int GuardianId { get; set; }
        public bool IsPrimary { get; set; }
        public string Status { get; set; } = null!;
    }

    public class UpdateStatusRequest
    {
        public string Status { get; set; } = null!;
    }
}
