namespace vangoplus_server.Application.DTOs
{
    /// <summary>
    /// One child's travel for one day: the roster row the operator marks off,
    /// and the line the parent reads.
    /// </summary>
    public class StudentDayStatusDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = null!;
        public int RouteId { get; set; }
        public string RouteName { get; set; } = null!;
        public DateOnly OnDate { get; set; }

        public DateTime? BoardedAt { get; set; }
        public DateTime? DroppedAt { get; set; }
    }
}
