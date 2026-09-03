using Microsoft.EntityFrameworkCore;
using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;
using vangoplus_server.Domain.Entities;
using vangoplus_server.Infrastructure.Data;

namespace vangoplus_server.Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly AppDbContext _db;

        public StudentService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<StudentDto> CreateAsync(StudentDto dto)
        {
            var entity = new Student
            {
                ParentUserId = dto.ParentUserId,
                Name = dto.Name,
                Grade = dto.Grade,
                Section = dto.Section,
                Status = dto.Status
            };

            _db.Students.Add(entity);
            await _db.SaveChangesAsync();

            dto.Id = entity.Id;
            return dto;
        }

        public async Task DeleteAsync(int id)
        {
            var e = await _db.Students.FindAsync(id);
            if (e == null) return;
            _db.Students.Remove(e);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<StudentDto>> GetAllAsync()
        {
            return await _db.Students.AsNoTracking()
                .Select(s => new StudentDto { Id = s.Id, ParentUserId = s.ParentUserId, Name = s.Name, Grade = s.Grade, Section = s.Section, Status = s.Status })
                .ToListAsync();
        }

        public async Task<StudentDto?> GetByIdAsync(int id)
        {
            var s = await _db.Students.FindAsync(id);
            if (s == null) return null;
            return new StudentDto { Id = s.Id, ParentUserId = s.ParentUserId, Name = s.Name, Grade = s.Grade, Section = s.Section, Status = s.Status };
        }

        public async Task UpdateAsync(int id, StudentDto dto)
        {
            var existing = await _db.Students.FindAsync(id);
            if (existing == null) return;
            existing.ParentUserId = dto.ParentUserId;
            existing.Name = dto.Name;
            existing.Grade = dto.Grade;
            existing.Section = dto.Section;
            existing.Status = dto.Status;
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<StudentDto>> GetByParentIdAsync(int parentId)
        {
            return await _db.Students.AsNoTracking()
                .Where(s => s.ParentUserId == parentId)
                .Select(s => new StudentDto { Id = s.Id, ParentUserId = s.ParentUserId, Name = s.Name, Grade = s.Grade, Section = s.Section, Status = s.Status })
                .ToListAsync();
        }
    }
}
