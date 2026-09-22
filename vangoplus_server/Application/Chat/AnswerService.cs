using System.Text;
using Microsoft.EntityFrameworkCore;
using vangoplus_server.Infrastructure.Data;

namespace vangoplus_server.Application.Chat
{
    public interface IAnswerService
    {
        Task<string> BuildAsync(string intent, ChatContext ctx);
    }

    /// <summary>
    /// Turns an intent plus the parent's own data into the reply text.
    /// Every branch has a "no data" answer — an empty result is an answer, not a crash.
    /// </summary>
    public class AnswerService : IAnswerService
    {
        private readonly AppDbContext _db;

        public AnswerService(AppDbContext db) => _db = db;

        // Postgres holds these as UTC. Pakistan is UTC+5 with no DST, so a raw timestamp
        // would show a clock five hours behind the parent's own.
        private static string D(DateTime utc) =>
            DateTime.SpecifyKind(utc, DateTimeKind.Utc).AddHours(5).ToString("d MMM yyyy, h:mm tt");

        private static string T(TimeSpan? t) =>
            t.HasValue ? DateTime.Today.Add(t.Value).ToString("h:mm tt") : "not set";

        public async Task<string> BuildAsync(string intent, ChatContext ctx) => intent switch
        {
            "pickup_time"         => Times(ctx, pickup: true),
            "dropoff_time"        => Times(ctx, pickup: false),
            "route_info"          => await RouteInfoAsync(ctx),
            "driver_info"         => DriverInfo(ctx),
            "child_status"        => await ChildStatusAsync(ctx),
            "van_location"        => VanLocation(ctx),
            "unread_alerts"       => await UnreadAlertsAsync(ctx),
            "subscription_status" => SubscriptionStatus(ctx),
            "plan_prices"         => await PlanPricesAsync(),
            "payment_history"     => PaymentHistory(ctx),
            "guardian_info"       => await GuardianInfoAsync(ctx),
            "subscribe_or_renew"  => await SubscribeOrRenewAsync(ctx),
            _                     => StaticAnswers.Answers.TryGetValue(intent, out var s) ? s : StaticAnswers.Fallback
        };

        private const string NoChildren =
            "You haven't added a child yet. Open My Children from the sidebar and tap \"Add Child\" to get started.";

        private static string NoRoute(string name) =>
            $"{name} hasn't been assigned a route yet, which is why the dashboard shows " +
            "\"No route assigned\". The transport admin assigns a route once the student is " +
            "approved, and you'll get an alert as soon as it's done. If it's been more than a " +
            "couple of days, please contact the school office.";

        private static string Times(ChatContext ctx, bool pickup)
        {
            if (!ctx.HasChildren) return NoChildren;

            var label = pickup ? "pickup" : "drop-off";
            var sb = new StringBuilder();

            foreach (var child in ctx.Children)
            {
                if (child.Assignment is null) { sb.AppendLine(NoRoute(child.Name)); continue; }

                var time = pickup ? child.Assignment.PickupTime : child.Assignment.DropoffTime;
                if (time is null)
                    sb.AppendLine($"{child.Name} is on {child.Assignment.RouteName}, but the {label} " +
                                  "time hasn't been set yet. Please contact the school office.");
                else
                    sb.AppendLine($"{child.Name} — {label} at {T(time)}, {child.Assignment.RouteName}.");
            }

            if (pickup && ctx.Children.Any(c => c.Assignment?.PickupTime is not null))
                sb.AppendLine("\nPlease have them at the stop 5 minutes early.");

            return sb.ToString().TrimEnd();
        }

        private async Task<string> RouteInfoAsync(ChatContext ctx)
        {
            if (!ctx.HasChildren) return NoChildren;

            var sb = new StringBuilder();
            foreach (var child in ctx.Children)
            {
                if (child.Assignment is null) { sb.AppendLine(NoRoute(child.Name)); continue; }

                var a = child.Assignment;
                sb.AppendLine($"{child.Name} is on {a.RouteName}" +
                              (string.IsNullOrWhiteSpace(a.RouteStatus) ? "." : $" ({a.RouteStatus})."));
                sb.AppendLine($"Pickup {T(a.PickupTime)} · Drop-off {T(a.DropoffTime)} · Driver: {a.DriverName}");

                var stops = await _db.RouteStops.AsNoTracking()
                    .Where(rs => rs.RouteId == a.RouteId)
                    .OrderBy(rs => rs.OrderIndex)
                    .Select(rs => new { rs.StopName, rs.ArrivalTime })
                    .ToListAsync();

                if (stops.Count > 0)
                {
                    sb.AppendLine("\nStops:");
                    var n = 1;
                    foreach (var s in stops)
                        sb.AppendLine($" {n++}. {s.StopName} — {T(s.ArrivalTime)}");
                }
                else
                {
                    sb.AppendLine("\nNo stops have been added to this route yet.");
                }
                sb.AppendLine();
            }
            return sb.ToString().TrimEnd();
        }

        private static string DriverInfo(ChatContext ctx)
        {
            if (!ctx.HasChildren) return NoChildren;

            var sb = new StringBuilder();
            foreach (var child in ctx.Children)
            {
                if (child.Assignment is null) { sb.AppendLine(NoRoute(child.Name)); continue; }

                var a = child.Assignment;
                sb.AppendLine($"{child.Name}'s driver is {a.DriverName}.");
                sb.AppendLine(string.IsNullOrWhiteSpace(a.DriverPhone)
                    ? "No phone number is on file for this driver — please call the school office."
                    : $"Phone: {a.DriverPhone}");
                if (!string.IsNullOrWhiteSpace(a.DriverLicense))
                    sb.AppendLine($"License: {a.DriverLicense}");
                sb.AppendLine();
            }

            // Vehicle/plate numbers are not columns on Driver, so we never invent one.
            sb.AppendLine("Vehicle registration numbers aren't recorded in the system yet — " +
                          "please ask the school office if you need one.");
            return sb.ToString().TrimEnd();
        }

        private async Task<string> ChildStatusAsync(ChatContext ctx)
        {
            if (!ctx.HasChildren) return NoChildren;

            var ids = ctx.ChildIds.ToList();
            var latest = await _db.Alerts.AsNoTracking()
                .Where(a => ids.Contains(a.StudentId))
                .GroupBy(a => a.StudentId)
                .Select(g => g.OrderByDescending(a => a.SentAt).First())
                .ToListAsync();

            var sb = new StringBuilder();
            foreach (var child in ctx.Children)
            {
                var alert = latest.FirstOrDefault(a => a.StudentId == child.Id);
                if (alert is not null)
                {
                    sb.AppendLine($"Latest update for {child.Name}:");
                    sb.AppendLine($"\"{alert.Title} — {alert.Message}\" ({D(alert.SentAt)})");
                }
                else
                {
                    sb.AppendLine($"There's no update recorded for {child.Name} yet. Live boarding " +
                                  "status isn't tracked in the system — the scheduled pickup is " +
                                  $"{T(child.Assignment?.PickupTime)}.");
                    if (!string.IsNullOrWhiteSpace(child.Assignment?.DriverPhone))
                        sb.AppendLine($"To check right now, call {child.Assignment.DriverName} on " +
                                      $"{child.Assignment.DriverPhone}.");
                }
                sb.AppendLine();
            }
            return sb.ToString().TrimEnd();
        }

        private static string VanLocation(ChatContext ctx)
        {
            if (!ctx.HasChildren) return NoChildren;

            var sb = new StringBuilder();
            sb.AppendLine("Live van tracking isn't available yet — it's planned for the next version.");
            foreach (var child in ctx.Children)
            {
                if (child.Assignment is null) continue;
                sb.AppendLine($"{child.Name}'s van is scheduled to reach your stop at " +
                              $"{T(child.Assignment.PickupTime)} on {child.Assignment.RouteName}.");
                if (!string.IsNullOrWhiteSpace(child.Assignment.DriverPhone))
                    sb.AppendLine($"If the van seems late, call {child.Assignment.DriverName} on " +
                                  $"{child.Assignment.DriverPhone}.");
            }
            return sb.ToString().TrimEnd();
        }

        private async Task<string> UnreadAlertsAsync(ChatContext ctx)
        {
            if (!ctx.HasChildren) return NoChildren;

            var ids = ctx.ChildIds.ToList();
            var unreadCount = await _db.Alerts.AsNoTracking()
                .CountAsync(a => ids.Contains(a.StudentId) && !a.IsRead);

            if (unreadCount == 0)
                return "You have no unread alerts. Everything's up to date.";

            var recent = await _db.Alerts.AsNoTracking()
                .Where(a => ids.Contains(a.StudentId) && !a.IsRead)
                .OrderByDescending(a => a.SentAt)
                .Take(5)
                .Select(a => new { a.Title, a.Message, a.SentAt, a.StudentId })
                .ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine($"You have {unreadCount} unread alert{(unreadCount == 1 ? "" : "s")}:");
            foreach (var a in recent)
            {
                var who = ctx.Children.FirstOrDefault(c => c.Id == a.StudentId)?.Name;
                sb.AppendLine($" • {a.Title} — {a.Message}" +
                              (who is null ? "" : $" ({who})") + $" · {D(a.SentAt)}");
            }
            if (unreadCount > recent.Count)
                sb.AppendLine($"\nOpen the Alerts page to see the other {unreadCount - recent.Count}.");
            return sb.ToString().TrimEnd();
        }

        private static string SubscriptionStatus(ChatContext ctx)
        {
            var s = ctx.Subscription;
            if (s is null)
                return "You don't have an active plan. Open Subscription → Choose a Plan to start " +
                       "van service.";

            var sb = new StringBuilder();
            sb.AppendLine($"Your plan: {s.PlanName} — Rs {s.Price:N0}/month");
            sb.AppendLine($"Status: {s.Status}");
            sb.AppendLine($"Started: {D(s.StartedAt)}");
            // Subscription has no end-date column, so this is stated as an estimate, not a fact.
            sb.AppendLine($"Next renewal: around {D(s.StartedAt.AddMonths(1))} (billed monthly)");
            sb.AppendLine($"Paid by: {s.PaymentMethod}");

            if (!string.Equals(s.Status, "active", StringComparison.OrdinalIgnoreCase))
                sb.AppendLine($"\nYour plan is currently marked \"{s.Status}\". Open the Subscription " +
                              "page to renew, or contact the office if that looks wrong.");
            return sb.ToString().TrimEnd();
        }

        private async Task<string> PlanPricesAsync()
        {
            var plans = await _db.SubscriptionPlans.AsNoTracking()
                .OrderBy(p => p.Price)
                .Select(p => new { p.Name, p.Price, p.MaxChildren, p.Features })
                .ToListAsync();

            if (plans.Count == 0)
                return "No plans have been set up yet. Please contact the school transport office.";

            var sb = new StringBuilder("Available plans:\n");
            foreach (var p in plans)
            {
                sb.Append($" • {p.Name} — Rs {p.Price:N0}/month · up to {p.MaxChildren} " +
                          $"child{(p.MaxChildren == 1 ? "" : "ren")}");
                sb.AppendLine(string.IsNullOrWhiteSpace(p.Features) ? "" : $" · {p.Features}");
            }
            sb.AppendLine("\nPick one from Subscription → Choose a Plan.");
            return sb.ToString().TrimEnd();
        }

        private static string PaymentHistory(ChatContext ctx)
        {
            if (ctx.SubscriptionHistory.Count == 0)
                return "There are no payment records on your account yet.";

            var sb = new StringBuilder("Your subscription records:\n");
            foreach (var s in ctx.SubscriptionHistory)
                sb.AppendLine($" • {s.PlanName} — Rs {s.Price:N0} — started {D(s.StartedAt)} — " +
                              $"{s.Status} — {s.PaymentMethod}");

            sb.AppendLine("\nItemised receipts and invoice downloads aren't available in the system " +
                          "yet. For a receipt, please contact the school office.");
            return sb.ToString().TrimEnd();
        }

        private async Task<string> GuardianInfoAsync(ChatContext ctx)
        {
            var guardians = await _db.Guardians.AsNoTracking()
                .Where(g => g.UserId == ctx.ParentUserId)
                .Select(g => new { g.Name, g.Relation, g.Phone, g.Status, g.Note })
                .ToListAsync();

            if (guardians.Count == 0)
                return "You haven't added any guardians yet, so only you can collect your child. " +
                       "Open Guardians → Add Guardian to authorise someone else.";

            var sb = new StringBuilder($"You have {guardians.Count} guardian" +
                                       $"{(guardians.Count == 1 ? "" : "s")}:\n");
            foreach (var g in guardians)
            {
                sb.AppendLine($" • {g.Name} ({g.Relation}) — {g.Phone ?? "no phone"} — " +
                              $"{g.Status ?? "pending"}");
                if (string.Equals(g.Status, "rejected", StringComparison.OrdinalIgnoreCase)
                    && !string.IsNullOrWhiteSpace(g.Note))
                    sb.AppendLine($"   Reason given: {g.Note}");
            }

            if (guardians.Any(g => string.Equals(g.Status, "pending", StringComparison.OrdinalIgnoreCase)))
                sb.AppendLine("\n\"Pending\" means the admin hasn't verified the CNIC yet — this " +
                              "usually takes 24–48 hours. Only approved guardians may collect a child.");

            return sb.ToString().TrimEnd();
        }

        private async Task<string> SubscribeOrRenewAsync(ChatContext ctx)
        {
            if (ctx.Subscription is null)
            {
                var plans = await _db.SubscriptionPlans.AsNoTracking()
                    .OrderBy(p => p.Price)
                    .Select(p => $"{p.Name} Rs {p.Price:N0}")
                    .ToListAsync();

                var sb = new StringBuilder("To start van service:\n");
                sb.AppendLine("1. Open Subscription from the sidebar");
                sb.AppendLine("2. Tap \"Choose a Plan\"");
                sb.AppendLine(plans.Count > 0
                    ? $"3. Pick a plan ({string.Join(" · ", plans)})"
                    : "3. Pick a plan");
                sb.AppendLine("4. Select your payment method and confirm");
                sb.AppendLine("\nAdd your children first under My Children, so the admin can assign " +
                              "them a route.");
                return sb.ToString().TrimEnd();
            }

            var s = ctx.Subscription;
            return $"You're on the {s.PlanName} plan (Rs {s.Price:N0}/month), started {D(s.StartedAt)}.\n" +
                   "To renew or change it, open Subscription → Choose a Plan and select a plan again. " +
                   "Renewing the same plan extends your cover; picking a different one switches you over.";
        }
    }
}
