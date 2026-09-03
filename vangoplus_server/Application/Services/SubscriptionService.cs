using Microsoft.EntityFrameworkCore;
using System.Data;
using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;
using vangoplus_server.Domain.Entities;
using vangoplus_server.Infrastructure.Data;

namespace vangoplus_server.Application.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly AppDbContext _db;

        public SubscriptionService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<SubscriptionDto> CreateAsync(SubscriptionDto dto)
        {
            var entity = new Subscription
            {
                UserId = dto.UserId,
                PlanId = dto.PlanId,
                PlanName = dto.PlanName,
                Price = dto.Price,
                Status = dto.Status,
                PaymentMethod = dto.PaymentMethod,
                StartedAt = DateTime.SpecifyKind(dto.StartedAt,DateTimeKind.Utc)
            };

            _db.Subscriptions.Add(entity);
            await _db.SaveChangesAsync();

            dto.Id = entity.Id;
            return dto;
        }

        public async Task<IEnumerable<SubscriptionDto>> GetAllAsync()
        {
            return await _db.Subscriptions.AsNoTracking()
                .Select(s => new SubscriptionDto
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    PlanId = s.PlanId,
                    PlanName = s.PlanName,
                    Price = s.Price,
                    Status = s.Status,
                    PaymentMethod = s.PaymentMethod,
                    StartedAt = s.StartedAt,
                    ParentName = _db.Users.Where(u => u.Id == s.UserId).Select(u => u.Name).FirstOrDefault(),
                    StudentName = _db.Students.Where(st => st.ParentUserId == s.UserId).Select(st => st.Name).FirstOrDefault()
                })
                .ToListAsync();
        }

        public async Task<SubscriptionDto?> GetByUserIdAsync(int userId)
        {
            var entity = await _db.Subscriptions.AsNoTracking()
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (entity == null) return null;

            var parentName = await _db.Users.AsNoTracking().Where(u => u.Id == entity.UserId).Select(u => u.Name).FirstOrDefaultAsync();
            var studentName = await _db.Students.AsNoTracking().Where(st => st.ParentUserId == entity.UserId).Select(st => st.Name).FirstOrDefaultAsync();

            return new SubscriptionDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                PlanId = entity.PlanId,
                PlanName = entity.PlanName,
                Price = entity.Price,
                Status = entity.Status,
                PaymentMethod = entity.PaymentMethod,
                StartedAt = entity.StartedAt,
                ParentName = parentName ?? string.Empty,
                StudentName = studentName ?? string.Empty
            };
        }

        public async Task<SubscriptionDto?> GetByIdAsync(int id)
        {
            var entity = await _db.Subscriptions.FindAsync(id);
            if (entity == null) return null;

            var parentName = await _db.Users.AsNoTracking().Where(u => u.Id == entity.UserId).Select(u => u.Name).FirstOrDefaultAsync();
            var studentName = await _db.Students.AsNoTracking().Where(st => st.ParentUserId == entity.UserId).Select(st => st.Name).FirstOrDefaultAsync();

            return new SubscriptionDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                PlanId = entity.PlanId,
                PlanName = entity.PlanName,
                Price = entity.Price,
                Status = entity.Status,
                PaymentMethod = entity.PaymentMethod,
                StartedAt = entity.StartedAt,
                ParentName = parentName ?? string.Empty,
                StudentName = studentName ?? string.Empty
            };
        }

        public async Task<SubscriptionDetailDto?> GetByIdWithUserAsync(int id)
        {
            var entity = await _db.Subscriptions.AsNoTracking()
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (entity == null) return null;

            return new SubscriptionDetailDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                PlanId = entity.PlanId,
                PlanName = entity.PlanName,
                Price = entity.Price,
                Status = entity.Status,
                PaymentMethod = entity.PaymentMethod,
                StartedAt = entity.StartedAt,
                User = entity.User != null ? new UserDto
                {
                    Id = entity.User.Id,
                    Name = entity.User.Name,
                    Email = entity.User.Email,
                    Phone = entity.User.Phone,
                    City = entity.User.City,
                    Role = entity.User.Role,
                    Status = entity.User.Status
                } : null
            };
        }

        public async Task UpdateAsync(int id, SubscriptionDto dto)
        {
            var existing = await _db.Subscriptions.FindAsync(id);
            if (existing == null) return;

            existing.UserId = dto.UserId;
            existing.PlanId = dto.PlanId;
            existing.PlanName = dto.PlanName;
            existing.Price = dto.Price;
            existing.Status = dto.Status;
            existing.PaymentMethod = dto.PaymentMethod;
            existing.StartedAt = dto.StartedAt;

            await _db.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int subscriptionId, string status)
        {
            var existing = await _db.Subscriptions.FindAsync(subscriptionId);
            if (existing == null)
                throw new InvalidOperationException("Subscription not found.");

            existing.Status = status;
            await _db.SaveChangesAsync();
        }
    }
}
