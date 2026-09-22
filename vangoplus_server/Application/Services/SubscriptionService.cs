using Microsoft.EntityFrameworkCore;
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

        // Create a new subscription OR update the latest subscription
        // for the same user to prevent duplicate current subscriptions.
        public async Task<SubscriptionDto> CreateAsync(SubscriptionDto dto)
        {
            var existing = await _db.Subscriptions
                .Where(s => s.UserId == dto.UserId)
                .OrderByDescending(s => s.Id)
                .FirstOrDefaultAsync();

            // No previous subscription exists
            if (existing == null)
            {
                var entity = new Subscription
                {
                    UserId = dto.UserId,
                    PlanId = dto.PlanId,
                    PlanName = dto.PlanName,
                    Price = dto.Price,
                    Status = dto.Status,
                    PaymentMethod = dto.PaymentMethod,

                    StartedAt = DateTime.SpecifyKind(
                        dto.StartedAt,
                        DateTimeKind.Utc
                    ),

                    // JazzCash fields
                    JazzCashNumber = dto.JazzCashNumber,
                    TransactionId = dto.TransactionId,
                    PaymentStatus = dto.PaymentStatus,
                    PaidAt = dto.PaidAt.HasValue
                        ? DateTime.SpecifyKind(
                            dto.PaidAt.Value,
                            DateTimeKind.Utc
                        )
                        : null
                };

                _db.Subscriptions.Add(entity);

                await _db.SaveChangesAsync();

                dto.Id = entity.Id;

                return dto;
            }

            // Existing subscription found.
            // Update latest subscription instead of creating duplicate.
            existing.UserId = dto.UserId;
            existing.PlanId = dto.PlanId;
            existing.PlanName = dto.PlanName;
            existing.Price = dto.Price;
            existing.Status = dto.Status;
            existing.PaymentMethod = dto.PaymentMethod;

            existing.StartedAt = DateTime.SpecifyKind(
                dto.StartedAt,
                DateTimeKind.Utc
            );

            // Only update JazzCash fields when values are supplied.
            // This prevents old payment information from being accidentally erased
            // by existing API requests that don't contain these fields.
            if (dto.JazzCashNumber != null)
                existing.JazzCashNumber = dto.JazzCashNumber;

            if (dto.TransactionId != null)
                existing.TransactionId = dto.TransactionId;

            if (dto.PaymentStatus != null)
                existing.PaymentStatus = dto.PaymentStatus;

            if (dto.PaidAt.HasValue)
            {
                existing.PaidAt = DateTime.SpecifyKind(
                    dto.PaidAt.Value,
                    DateTimeKind.Utc
                );
            }

            await _db.SaveChangesAsync();

            dto.Id = existing.Id;

            return dto;
        }


        // Get all subscriptions
        public async Task<IEnumerable<SubscriptionDto>> GetAllAsync()
        {
            return await _db.Subscriptions
                .AsNoTracking()
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

                    // JazzCash fields
                    JazzCashNumber = s.JazzCashNumber,
                    TransactionId = s.TransactionId,
                    PaymentStatus = s.PaymentStatus,
                    PaidAt = s.PaidAt,

                    ParentName = _db.Users
                        .Where(u => u.Id == s.UserId)
                        .Select(u => u.Name)
                        .FirstOrDefault(),

                    StudentName = _db.Students
                        .Where(st => st.ParentUserId == s.UserId)
                        .Select(st => st.Name)
                        .FirstOrDefault()
                })
                .ToListAsync();
        }


        // Get latest subscription for a specific user
        public async Task<SubscriptionDto?> GetByUserIdAsync(int userId)
        {
            var entity = await _db.Subscriptions
                .AsNoTracking()
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.Id)
                .FirstOrDefaultAsync();

            if (entity == null)
                return null;

            var parentName = await _db.Users
                .AsNoTracking()
                .Where(u => u.Id == entity.UserId)
                .Select(u => u.Name)
                .FirstOrDefaultAsync();

            var studentName = await _db.Students
                .AsNoTracking()
                .Where(st => st.ParentUserId == entity.UserId)
                .Select(st => st.Name)
                .FirstOrDefaultAsync();

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

                // JazzCash fields
                JazzCashNumber = entity.JazzCashNumber,
                TransactionId = entity.TransactionId,
                PaymentStatus = entity.PaymentStatus,
                PaidAt = entity.PaidAt,

                ParentName = parentName ?? string.Empty,
                StudentName = studentName ?? string.Empty
            };
        }


        // Get subscription by ID
        public async Task<SubscriptionDto?> GetByIdAsync(int id)
        {
            var entity = await _db.Subscriptions
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (entity == null)
                return null;

            var parentName = await _db.Users
                .AsNoTracking()
                .Where(u => u.Id == entity.UserId)
                .Select(u => u.Name)
                .FirstOrDefaultAsync();

            var studentName = await _db.Students
                .AsNoTracking()
                .Where(st => st.ParentUserId == entity.UserId)
                .Select(st => st.Name)
                .FirstOrDefaultAsync();

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

                // JazzCash fields
                JazzCashNumber = entity.JazzCashNumber,
                TransactionId = entity.TransactionId,
                PaymentStatus = entity.PaymentStatus,
                PaidAt = entity.PaidAt,

                ParentName = parentName ?? string.Empty,
                StudentName = studentName ?? string.Empty
            };
        }


        // Get subscription with complete user details
        public async Task<SubscriptionDetailDto?> GetByIdWithUserAsync(int id)
        {
            var entity = await _db.Subscriptions
                .AsNoTracking()
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (entity == null)
                return null;

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

                User = entity.User != null
                    ? new UserDto
                    {
                        Id = entity.User.Id,
                        Name = entity.User.Name,
                        Email = entity.User.Email,
                        Phone = entity.User.Phone,
                        City = entity.User.City,
                        Role = entity.User.Role,
                        Status = entity.User.Status
                    }
                    : null
            };
        }


        // Update subscription by ID
        public async Task UpdateAsync(int id, SubscriptionDto dto)
        {
            var existing = await _db.Subscriptions
                .FindAsync(id);

            if (existing == null)
                return;

            existing.UserId = dto.UserId;
            existing.PlanId = dto.PlanId;
            existing.PlanName = dto.PlanName;
            existing.Price = dto.Price;
            existing.Status = dto.Status;
            existing.PaymentMethod = dto.PaymentMethod;

            existing.StartedAt = DateTime.SpecifyKind(
                dto.StartedAt,
                DateTimeKind.Utc
            );

            // JazzCash fields
            if (dto.JazzCashNumber != null)
                existing.JazzCashNumber = dto.JazzCashNumber;

            if (dto.TransactionId != null)
                existing.TransactionId = dto.TransactionId;

            if (dto.PaymentStatus != null)
                existing.PaymentStatus = dto.PaymentStatus;

            if (dto.PaidAt.HasValue)
            {
                existing.PaidAt = DateTime.SpecifyKind(
                    dto.PaidAt.Value,
                    DateTimeKind.Utc
                );
            }

            await _db.SaveChangesAsync();
        }


        // Update only subscription status
        public async Task UpdateStatusAsync(
            int subscriptionId,
            string status)
        {
            var existing = await _db.Subscriptions
                .FindAsync(subscriptionId);

            if (existing == null)
            {
                throw new InvalidOperationException(
                    "Subscription not found."
                );
            }

            existing.Status = status;

            await _db.SaveChangesAsync();
        }
    }
}