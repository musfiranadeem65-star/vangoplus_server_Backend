using Microsoft.EntityFrameworkCore;
using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;
using vangoplus_server.Domain.Entities;
using vangoplus_server.Infrastructure.Data;

namespace vangoplus_server.Application.Services
{
    public class DriverService : IDriverService
    {
        private readonly AppDbContext _db;

        public DriverService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<DriverDto> CreateAsync(DriverDto dto)
        {
            var entity = new Driver
            {
                Name = dto.Name,
                Phone = dto.Phone,
                Email = dto.Email,
                LicenseNo = dto.LicenseNo,
                Status = dto.Status
            };

            _db.Drivers.Add(entity);
            await _db.SaveChangesAsync();

            dto.Id = entity.Id;
            return dto;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.Drivers.FindAsync(id);
            if (entity == null) return;
            _db.Drivers.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<DriverDto>> GetAllAsync()
        {
            return await _db.Drivers.AsNoTracking()
                .Select(d => new DriverDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Phone = d.Phone,
                    Email = d.Email,
                    LicenseNo = d.LicenseNo,
                    Status = d.Status
                })
                .ToListAsync();
        }

        public async Task<DriverDto?> GetByIdAsync(int id)
        {
            var entity = await _db.Drivers.FindAsync(id);
            if (entity == null) return null;
            return new DriverDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Phone = entity.Phone,
                Email = entity.Email,
                LicenseNo = entity.LicenseNo,
                Status = entity.Status
            };
        }

        public async Task UpdateAsync(int id, DriverDto dto)
        {
            var existing = await _db.Drivers.FindAsync(id);
            if (existing == null) return;
            existing.Name = dto.Name;
            existing.Phone = dto.Phone;
            existing.Email = dto.Email;
            existing.LicenseNo = dto.LicenseNo;
            existing.Status = dto.Status;
            await _db.SaveChangesAsync();
        }
        public async Task UpdateStatusAsync(int driverId, string status)
        {
            var existing = await _db.Drivers.FindAsync(driverId);

            if (existing == null)
                throw new InvalidOperationException("Driver not found.");

            existing.Status = status;

            await _db.SaveChangesAsync();
        }
    }
}
