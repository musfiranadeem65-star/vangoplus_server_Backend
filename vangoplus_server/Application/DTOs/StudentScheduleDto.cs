namespace vangoplus_server.Application.DTOs
{
    /// <summary>
    /// Everything the parent's Transport Schedule screen needs for one child:
    /// which route they are on, who drives it, and the stops in order.
    /// </summary>
    public class StudentScheduleDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = null!;

        public int? RouteId { get; set; }
        public string? RouteName { get; set; }
        public int? DriverId { get; set; }
        public string? DriverName { get; set; }

        public TimeSpan? PickupTime { get; set; }
        public TimeSpan? DropoffTime { get; set; }

        public List<RouteStopDto> RouteStops { get; set; } = new();
    }
}
