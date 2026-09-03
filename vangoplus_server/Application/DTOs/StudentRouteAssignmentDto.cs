namespace vangoplus_server.Application.DTOs
{
    public class StudentRouteAssignmentDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int RouteId { get; set; }
        public TimeSpan? PickupTime { get; set; }
        public TimeSpan? DropoffTime { get; set; }
        public DateTime AssignedAt { get; set; }
        public string Status { get; set; } = null!;
    }
}
