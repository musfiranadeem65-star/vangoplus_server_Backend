using Microsoft.EntityFrameworkCore;
using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;
using vangoplus_server.Domain.Entities;
using vangoplus_server.Infrastructure.Data;

namespace vangoplus_server.Application.Services
{
    public class GuardianService : IGuardianService
    {

        private readonly AppDbContext _db;

        public GuardianService(AppDbContext db)
        {
            _db = db;
        }
      

        public async Task<GuardianDto> CreateAsync(GuardianDto dto)
        {
            var entity = new Guardian
            {
                UserId = dto.UserId,
                Name = dto.Name,
                Relation = dto.Relation,
                Phone = dto.Phone,
                Status = dto.Status,
                Note = dto.Note,
                IdentityDocumentPath = dto.IdentityDocumentPath
            };

            _db.Guardians.Add(entity);
            await _db.SaveChangesAsync();

            dto.Id = entity.Id;
            return dto;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.Guardians.FindAsync(id);
            if (entity == null) return;
            _db.Guardians.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<GuardianDto>> GetAllAsync()
        {
            return await _db.Guardians.AsNoTracking()
                .Select(g => new GuardianDto 
                { 
                    Id = g.Id, 
                    UserId = g.UserId, 
                    Name = g.Name, 
                    Relation = g.Relation, 
                    Phone = g.Phone, 
                    Status = g.Status, 
                    Note = g.Note,
                    IdentityDocumentPath = g.IdentityDocumentPath
                })
                .ToListAsync();
        }

        public async Task<GuardianDto?> GetByIdAsync(int id)
        {
            var entity = await _db.Guardians.FindAsync(id);
            if (entity == null) return null;
            return new GuardianDto 
            { 
                Id = entity.Id, 
                UserId = entity.UserId, 
                Name = entity.Name, 
                Relation = entity.Relation, 
                Phone = entity.Phone, 
                Status = entity.Status, 
                Note = entity.Note,
                IdentityDocumentPath = entity.IdentityDocumentPath
            };
        }

        public async Task UpdateAsync(int id, GuardianDto dto)
        {
            var existing = await _db.Guardians.FindAsync(id);
            if (existing == null) return;
            existing.UserId = dto.UserId;
            existing.Name = dto.Name;
            existing.Relation = dto.Relation;
            existing.Phone = dto.Phone;
            existing.Status = dto.Status;
            existing.Note = dto.Note;
            // Update IdentityDocumentPath only when provided (do not erase existing path on empty submission)
            if (!string.IsNullOrWhiteSpace(dto.IdentityDocumentPath))
            {
                existing.IdentityDocumentPath = dto.IdentityDocumentPath;
            }
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<GuardianDto>> GetByStudentIdAsync(int studentId)
        {
            return await _db.StudentGuardians.AsNoTracking()
                .Where(sg => sg.StudentId == studentId)
                .Join(_db.Guardians.AsNoTracking(), 
                    sg => sg.GuardianId, 
                    g => g.Id, 
                    (sg, g) => new GuardianDto 
                    { 
                        Id = g.Id, 
                        UserId = g.UserId, 
                        Name = g.Name, 
                        Relation = g.Relation, 
                        Phone = g.Phone, 
                        Status = g.Status, 
                        Note = g.Note,
                        IdentityDocumentPath = g.IdentityDocumentPath 
                    })
                .ToListAsync();
        }

        public async Task LinkToStudentAsync(int studentId, int guardianId, StudentGuardianDto linkDto)
        {
            var exists = await _db.StudentGuardians.AnyAsync(x => x.StudentId == studentId && x.GuardianId == guardianId);
            if (exists)
                    throw new InvalidOperationException("Guardian is already linked to this student.");
            var entity = new StudentGuardian
            {
                StudentId = studentId,
                GuardianId = guardianId,
                IsPrimary = linkDto.IsPrimary,
                Status = linkDto.Status
            };

            _db.StudentGuardians.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int guardianId, string status)
        {
            var existing = await _db.Guardians.FindAsync(guardianId);
            if (existing == null) return;
            existing.Status = status;
            await _db.SaveChangesAsync();
        }
    }
}
