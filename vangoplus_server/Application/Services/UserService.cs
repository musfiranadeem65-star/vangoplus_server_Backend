using Microsoft.EntityFrameworkCore;
using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;
using vangoplus_server.Domain.Entities;
using vangoplus_server.Infrastructure.Data;
using System.Security.Cryptography;
using System.Text;

namespace vangoplus_server.Application.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;

public UserService(AppDbContext db)
{
    _db = db;
}

        public async Task<UserDto> CreateAsync(UserDto dto, string passwordPlain)
        {
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = HashPassword(passwordPlain),
                Phone = dto.Phone,
                City = dto.City,
                Role = dto.Role,
                Status = dto.Status
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            dto.Id = user.Id;
            return dto;
        }

        public async Task DeleteAsync(int id)
        {
            var e = await _db.Users.FindAsync(id);
            if (e == null) return;
            _db.Users.Remove(e);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            return await _db.Users.AsNoTracking()
                .Select(u => new UserDto { Id = u.Id, Name = u.Name, Email = u.Email, Phone = u.Phone, City = u.City, Role = u.Role, Status = u.Status })
                .ToListAsync();
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var u = await _db.Users.FindAsync(id);
            if (u == null) return null;
            return new UserDto { Id = u.Id, Name = u.Name, Email = u.Email, Phone = u.Phone, City = u.City, Role = u.Role, Status = u.Status };
        }

        public async Task<UserDetailDto?> GetByIdWithSubscriptionsAsync(int id)
        {
            var u = await _db.Users.AsNoTracking()
                .Include(u => u.Subscriptions)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (u == null) return null;

            return new UserDetailDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Phone = u.Phone,
                City = u.City,
                Role = u.Role,
                Status = u.Status,
                CreatedAt = u.CreatedAt,
                Subscriptions = u.Subscriptions
                    .Select(s => new SubscriptionDto
                    {
                        Id = s.Id,
                        UserId = s.UserId,
                        PlanId = s.PlanId,
                        PlanName = s.PlanName,
                        Price = s.Price,
                        Status = s.Status,
                        PaymentMethod = s.PaymentMethod,
                        StartedAt = s.StartedAt
                    })
                    .ToList()
            };
        }

        public async Task UpdateAsync(int id, UserDto dto)
        {
            var existing = await _db.Users.FindAsync(id);
            if (existing == null) return;
            existing.Name = dto.Name;
            existing.Email = dto.Email;
            existing.Phone = dto.Phone;
            existing.City = dto.City;
            existing.Role = dto.Role;
            existing.Status = dto.Status;
            await _db.SaveChangesAsync();
        }

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            var u = await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email);
            if (u == null) return null;
            return new UserDto { Id = u.Id, Name = u.Name, Email = u.Email, Phone = u.Phone, City = u.City, Role = u.Role, Status = u.Status };
        }

        public async Task<UserDto?> LoginAsync(string email, string passwordPlain)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(passwordPlain))
                return null;

var normalizedEmail = email.Trim().ToLower();
            var u = await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email.ToLower() == normalizedEmail);
            if (u == null) return null;

var hash = HashPassword(passwordPlain);
            if (u.PasswordHash != hash) return null;

return new UserDto { Id = u.Id, Name = u.Name, Email = u.Email, Phone = u.Phone, City = u.City, Role = u.Role, Status = u.Status };
        }

        public async Task ChangePasswordAsync(ChangePasswordDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.CurrentPassword) || string.IsNullOrWhiteSpace(dto.NewPassword))
                throw new ArgumentException("Email, current password and new password are required.");

            var normalizedEmail = dto.Email.Trim().ToLower();
            var u = await _db.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == normalizedEmail);
            if (u == null) throw new InvalidOperationException("User not found.");

            var currentHash = HashPassword(dto.CurrentPassword);
            if (u.PasswordHash != currentHash) throw new InvalidOperationException("Current password is incorrect.");

            u.PasswordHash = HashPassword(dto.NewPassword);
            await _db.SaveChangesAsync();
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public async Task<NotificationPreferencesDto?> GetNotificationPreferencesAsync(int id)
        {
            var u = await _db.Users.FindAsync(id);
            if (u == null) return null;
            return new NotificationPreferencesDto
            {
                EmailAlerts = u.EmailAlerts,
                SmsAlerts = u.SmsAlerts
            };
        }

        public async Task UpdateNotificationPreferencesAsync(int id, NotificationPreferencesDto dto)
        {
            var u = await _db.Users.FindAsync(id);
            if (u == null) return;
            u.EmailAlerts = dto.EmailAlerts;
            u.SmsAlerts = dto.SmsAlerts;
            await _db.SaveChangesAsync();
        }
    }
}
