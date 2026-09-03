using Microsoft.EntityFrameworkCore;
using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;
using vangoplus_server.Domain.Entities;
using vangoplus_server.Infrastructure.Data;

namespace vangoplus_server.Application.Services
{
    public class SubscriptionPlanService : ISubscriptionPlanService
    {
        private readonly AppDbContext _db;

        public SubscriptionPlanService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<SubscriptionPlanDto> CreateAsync(SubscriptionPlanDto dto)
        {
            var entity = new SubscriptionPlan
            {
                Name = dto.Name,
                Price = dto.Price,
                MaxChildren = dto.MaxChildren,
                Features = dto.Features
            };

            _db.SubscriptionPlans.Add(entity);
            await _db.SaveChangesAsync();

            dto.Id = entity.Id;
            return dto;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.SubscriptionPlans.FindAsync(id);
            if (entity == null) return;
            _db.SubscriptionPlans.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<SubscriptionPlanDto>> GetAllAsync()
        {
            return await _db.SubscriptionPlans.AsNoTracking()
                .Select(sp => new SubscriptionPlanDto
                {
                    Id = sp.Id,
                    Name = sp.Name,
                    Price = sp.Price,
                    MaxChildren = sp.MaxChildren,
                    Features = sp.Features
                })
                .ToListAsync();
        }

        public async Task<SubscriptionPlanDto?> GetByIdAsync(int id)
        {
            var entity = await _db.SubscriptionPlans.FindAsync(id);
            if (entity == null) return null;
            return new SubscriptionPlanDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Price = entity.Price,
                MaxChildren = entity.MaxChildren,
                Features = entity.Features
            };
        }

        public async Task UpdateAsync(int id, SubscriptionPlanDto dto)
        {
            var existing = await _db.SubscriptionPlans.FindAsync(id);
            if (existing == null) return;
            existing.Name = dto.Name;
            existing.Price = dto.Price;
            existing.MaxChildren = dto.MaxChildren;
            existing.Features = dto.Features;
            await _db.SaveChangesAsync();
        }
    }
}
