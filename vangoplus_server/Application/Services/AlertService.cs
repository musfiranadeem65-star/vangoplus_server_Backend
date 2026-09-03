using Microsoft.EntityFrameworkCore;
using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;
using vangoplus_server.Domain.Entities;
using vangoplus_server.Infrastructure.Data;

namespace vangoplus_server.Application.Services
{
    public class AlertService : IAlertService
    {
        private readonly AppDbContext _db;

        public AlertService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<AlertDto> CreateAsync(AlertDto dto)
        {
            var entity = new Alert
            {
                StudentId = dto.StudentId,
                Type = dto.Type,
                Title = dto.Title,
                Message = dto.Message,
                SentAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
                IsRead = false
            };

            _db.Alerts.Add(entity);
            await _db.SaveChangesAsync();

            dto.Id = entity.Id;
            dto.SentAt = entity.SentAt;
            dto.IsRead = entity.IsRead;
            return dto;
        }

        public async Task<IEnumerable<AlertDto>> GetByStudentIdAsync(int studentId)
        {
            return await _db.Alerts.AsNoTracking()
                .Where(a => a.StudentId == studentId)
                .Select(a => new AlertDto
                {
                    Id = a.Id,
                    StudentId = a.StudentId,
                    Type = a.Type,
                    Title = a.Title,
                    Message = a.Message,
                    SentAt = a.SentAt,
                    IsRead = a.IsRead
                })
                .ToListAsync();
        }

        public async Task<AlertDto?> GetByIdAsync(int id)
        {
            var entity = await _db.Alerts.FindAsync(id);
            if (entity == null) return null;

            return new AlertDto
            {
                Id = entity.Id,
                StudentId = entity.StudentId,
                Type = entity.Type,
                Title = entity.Title,
                Message = entity.Message,
                SentAt = entity.SentAt,
                IsRead = entity.IsRead
            };
        }

        public async Task UpdateReadStatusAsync(int alertId)
        {
            var existing = await _db.Alerts.FindAsync(alertId);
            if (existing == null)
                throw new InvalidOperationException("Alert not found.");

            existing.IsRead = true;
            await _db.SaveChangesAsync();
        }
    }
}
