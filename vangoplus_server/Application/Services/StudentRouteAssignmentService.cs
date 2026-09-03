using Microsoft.EntityFrameworkCore;
using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;
using vangoplus_server.Domain.Entities;
using vangoplus_server.Infrastructure.Data;

namespace vangoplus_server.Application.Services
{
    public class StudentRouteAssignmentService : IStudentRouteAssignmentService
    {
        private readonly AppDbContext _db;

        public StudentRouteAssignmentService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<StudentRouteAssignmentDto> CreateAsync(StudentRouteAssignmentDto dto)
        {
            var entity = new StudentRouteAssignment
            {
                StudentId = dto.StudentId,
                RouteId = dto.RouteId,
                PickupTime = dto.PickupTime,
                DropoffTime = dto.DropoffTime,
                AssignedAt = DateTime.SpecifyKind(dto.AssignedAt, DateTimeKind.Utc),
                Status = dto.Status
            };

            _db.StudentRouteAssignments.Add(entity);
            await _db.SaveChangesAsync();

            dto.Id = entity.Id;
            return dto;
        }

        public async Task<IEnumerable<StudentRouteAssignmentDto>> GetAllAsync()
        {
            return await _db.StudentRouteAssignments.AsNoTracking()
                .Select(sra => new StudentRouteAssignmentDto
                {
                    Id = sra.Id,
                    StudentId = sra.StudentId,
                    RouteId = sra.RouteId,
                    PickupTime = sra.PickupTime,
                    DropoffTime = sra.DropoffTime,
                    AssignedAt = sra.AssignedAt,
                    Status = sra.Status
                })
                .ToListAsync();
        }

        public async Task<StudentRouteAssignmentDto?> GetByIdAsync(int id)
        {
            return await _db.StudentRouteAssignments.AsNoTracking()
                .Where(sra => sra.Id == id)
                .Select(sra => new StudentRouteAssignmentDto
                {
                    Id = sra.Id,
                    StudentId = sra.StudentId,
                    RouteId = sra.RouteId,
                    PickupTime = sra.PickupTime,
                    DropoffTime = sra.DropoffTime,
                    AssignedAt = sra.AssignedAt,
                    Status = sra.Status
                })
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(int id, StudentRouteAssignmentDto dto)
        {
            var entity = await _db.StudentRouteAssignments.FindAsync(id);
            if (entity == null)
                throw new InvalidOperationException($"StudentRouteAssignment with Id {id} not found.");

            entity.StudentId = dto.StudentId;
            entity.RouteId = dto.RouteId;
            entity.PickupTime = dto.PickupTime;
            entity.DropoffTime = dto.DropoffTime;
            entity.AssignedAt = DateTime.SpecifyKind(dto.AssignedAt, DateTimeKind.Utc);
            entity.Status = dto.Status;

            _db.StudentRouteAssignments.Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int id, string status)
        {
            var entity = await _db.StudentRouteAssignments.FindAsync(id);
            if (entity == null)
                throw new InvalidOperationException($"StudentRouteAssignment with Id {id} not found.");

            entity.Status = status;
            _db.StudentRouteAssignments.Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.StudentRouteAssignments.FindAsync(id);
            if (entity == null)
                throw new InvalidOperationException($"StudentRouteAssignment with Id {id} not found.");

            _db.StudentRouteAssignments.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
