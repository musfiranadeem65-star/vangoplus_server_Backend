using vangoplus_server.Domain.Entities;
using vangoplus_server.Infrastructure.Data;

namespace vangoplus_server.Application.Chat
{
    public interface IEscalationService
    {
        Task<string> HandleAsync(string intent, string originalMessage, ChatContext ctx);
        bool IsEscalation(string intent);
    }

    /// <summary>
    /// The four intents that are requests rather than questions. Each one writes an Alert row
    /// so the admin actually sees it, storing the parent's own words — the intent label alone
    /// tells the office nothing useful.
    /// </summary>
    public class EscalationService : IEscalationService
    {
        private static readonly HashSet<string> Escalations = new(StringComparer.OrdinalIgnoreCase)
        { "report_absence", "change_route_request", "complaint", "emergency" };

        private readonly AppDbContext _db;

        public EscalationService(AppDbContext db) => _db = db;

        public bool IsEscalation(string intent) => Escalations.Contains(intent);

        public async Task<string> HandleAsync(string intent, string originalMessage, ChatContext ctx)
        {
            var child = ctx.Children.FirstOrDefault();

            if (child is not null)
            {
                _db.Alerts.Add(new Alert
                {
                    StudentId = child.Id,
                    Type      = intent,
                    Title     = TitleFor(intent),
                    Message   = originalMessage,
                    SentAt    = DateTime.UtcNow,
                    IsRead    = false
                });
                await _db.SaveChangesAsync();
            }

            var driver = child?.Assignment;
            var driverLine = driver is null || string.IsNullOrWhiteSpace(driver.DriverPhone)
                ? "Please also contact the school transport office."
                : $"Driver: {driver.DriverName} — {driver.DriverPhone}";

            return intent switch
            {
                "report_absence" =>
                    $"Noted — I've sent an absence request to the transport office" +
                    (child is null ? "." : $" for {child.Name}.") + "\n" +
                    "Please also message the driver directly so the van doesn't wait:\n" +
                    driverLine + "\n\n" +
                    "There's no automatic absence system yet, so the office confirms these by hand.",

                "change_route_request" =>
                    "Route, stop and timing changes are made by the transport admin, so I've passed " +
                    "your request on. Please contact the school office with the details to confirm — " +
                    "changes usually take effect from the next working day.",

                "complaint" =>
                    "I'm sorry about that. I've recorded your complaint and sent it to the transport " +
                    "office, and someone will follow up with you. If this is about your child's " +
                    "immediate safety, please call the office right away rather than waiting for a reply.",

                "emergency" =>
                    "If your child is in danger, call 15 (Police) or 1122 (Rescue) now.\n\n" +
                    "I've flagged this to the transport office as urgent.\n" +
                    driverLine + "\n\n" +
                    "Please call rather than wait for a reply here — this chat isn't monitored live.",

                _ => StaticAnswers.Fallback
            };
        }

        private static string TitleFor(string intent) => intent switch
        {
            "report_absence"       => "Absence reported via chat",
            "change_route_request" => "Route change requested via chat",
            "complaint"            => "Complaint submitted via chat",
            "emergency"            => "EMERGENCY reported via chat",
            _                      => "Parent request via chat"
        };
    }
}
