using Microsoft.EntityFrameworkCore;
using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;
using vangoplus_server.Domain.Entities;
using vangoplus_server.Infrastructure.Data;

namespace vangoplus_server.Application.Services
{
    public class SchoolSettingService : ISchoolSettingService
    {
        private readonly AppDbContext _db;

        public SchoolSettingService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<SchoolSettingDto> GetAsync()
        {
            var entity = await _db.SchoolSettings.AsNoTracking().FirstOrDefaultAsync();
            if (entity == null)
            {
                entity = new SchoolSetting
                {
                    SchoolName = string.Empty,
                    ContactPerson = string.Empty,
                    SchoolAddress = string.Empty,
                    MonthlyAmount = 0m,
                    SenderEmail = string.Empty
                };
                _db.SchoolSettings.Add(entity);
                await _db.SaveChangesAsync();
            }

            return new SchoolSettingDto
            {
                Id = entity.Id,
                SchoolName = entity.SchoolName,
                ContactPerson = entity.ContactPerson,
                SchoolAddress = entity.SchoolAddress,
                MonthlyAmount = entity.MonthlyAmount,
                SenderEmail = entity.SenderEmail
            };
        }

        public async Task<SchoolSettingDto> UpdateAsync(UpdateSchoolSettingDto dto)
        {
            var entity = await _db.SchoolSettings.FirstOrDefaultAsync();
            if (entity == null)
            {
                entity = new SchoolSetting();
                _db.SchoolSettings.Add(entity);
            }

            entity.SchoolName = dto.SchoolName;
            entity.ContactPerson = dto.ContactPerson;
            entity.SchoolAddress = dto.SchoolAddress;
            entity.MonthlyAmount = dto.MonthlyAmount;
            entity.SenderEmail = dto.SenderEmail;

            await _db.SaveChangesAsync();

            return new SchoolSettingDto
            {
                Id = entity.Id,
                SchoolName = entity.SchoolName,
                ContactPerson = entity.ContactPerson,
                SchoolAddress = entity.SchoolAddress,
                MonthlyAmount = entity.MonthlyAmount,
                SenderEmail = entity.SenderEmail
            };
        }
    }
}
