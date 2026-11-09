namespace vangoplus_server.Application.DTOs
{
    public class TripRecordDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = null!;
        public int RouteId { get; set; }
        public string RouteName { get; set; } = null!;
        public string Kind { get; set; } = null!;
        public DateTime OccurredAt { get; set; }
        public DateOnly OnDate { get; set; }
        public string? Note { get; set; }
    }
}
