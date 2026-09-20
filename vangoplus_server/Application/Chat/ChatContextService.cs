using Microsoft.EntityFrameworkCore;
using vangoplus_server.Infrastructure.Data;

namespace vangoplus_server.Application.Chat
{
    public class AssignmentContext
    {
        public TimeSpan? PickupTime { get; set; }
        public TimeSpan? DropoffTime { get; set; }
        public string? Status { get; set; }
        public int RouteId { get; set; }
        public string RouteName { get; set; } = null!;
        public string? RouteStatus { get; set; }
        public string DriverName { get; set; } = null!;
        public string? DriverPhone { get; set; }
        public string? DriverLicense { get; set; }
    }

    public class ChildContext
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Grade { get; set; }
        public string? Section { get; set; }
        public string? Status { get; set; }
        public AssignmentContext? Assignment { get; set; }
    }

    public class SubscriptionContext
    {
        public string PlanName { get; set; } = null!;
        public decimal Price { get; set; }
        public string Status { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
        public DateTime StartedAt { get; set; }
    }

    public class ChatContext
    {
        public int ParentUserId { get; set; }
        public string ParentName { get; set; } = string.Empty;
        public List<ChildContext> Children { get; set; } = new();
        public SubscriptionContext? Subscription { get; set; }
        public List<SubscriptionContext> SubscriptionHistory { get; set; } = new();

        public bool HasChildren => Children.Count > 0;
        public IEnumerable<int> ChildIds => Children.Select(c => c.Id);
    }

    /// <summary>
    /// Loads everything the answer templates need, in one place, always scoped to the
    /// signed-in parent. No chat query may take a student id from the message — a parent
    /// must never be able to read another family's data by guessing an id.
    /// </summary>
    public class ChatContextService : IChatContextService
    {
        private readonly AppDbContext _db;

        public ChatContextService(AppDbContext db) => _db = db;

        public async Task<ChatContext> LoadAsync(int parentUserId)
        {
            var children = await _db.Students.AsNoTracking()
                .Where(s => s.ParentUserId == parentUserId)
                .Select(s => new ChildContext
                {
                    Id = s.Id,
                    Name = s.Name,
                    Grade = s.Grade,
                    Section = s.Section,
                    Status = s.Status,
                    Assignment = _db.StudentRouteAssignments.AsNoTracking()
                        .Where(a => a.StudentId == s.Id)
                        .OrderByDescending(a => a.AssignedAt)
                        .Select(a => new AssignmentContext
                        {
                            PickupTime = a.PickupTime,
                            DropoffTime = a.DropoffTime,
                            Status = a.Status,
                            RouteId = a.RouteId,
                            RouteName = a.Route.Name,
                            RouteStatus = a.Route.Status,
                            DriverName = a.Route.Driver.Name,
                            DriverPhone = a.Route.Driver.Phone,
                            DriverLicense = a.Route.Driver.LicenseNo
                        })
                        .FirstOrDefault()
                })
                .ToListAsync();

            var subs = await _db.Subscriptions.AsNoTracking()
                .Where(s => s.UserId == parentUserId)
                .OrderByDescending(s => s.StartedAt)
                .Select(s => new SubscriptionContext
                {
                    PlanName = s.PlanName,
                    Price = s.Price,
                    Status = s.Status,
                    PaymentMethod = s.PaymentMethod,
                    StartedAt = s.StartedAt
                })
                .ToListAsync();

            var parentName = await _db.Users.AsNoTracking()
                .Where(u => u.Id == parentUserId)
                .Select(u => u.Name)
                .FirstOrDefaultAsync() ?? string.Empty;

            return new ChatContext
            {
                ParentUserId = parentUserId,
                ParentName = parentName,
                Children = children,
                Subscription = subs.FirstOrDefault(),
                SubscriptionHistory = subs
            };
        }
    }

    public interface IChatContextService
    {
        Task<ChatContext> LoadAsync(int parentUserId);
    }
}
