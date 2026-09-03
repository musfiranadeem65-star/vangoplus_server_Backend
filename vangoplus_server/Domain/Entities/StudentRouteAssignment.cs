namespace vangoplus_server.Domain.Entities
{
    public class StudentRouteAssignment
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int RouteId { get; set; }
        public TimeSpan? PickupTime { get; set; }
        public TimeSpan? DropoffTime { get; set; }
        public DateTime AssignedAt { get; set; }
        public string Status { get; set; } = null!;

        // Navigation
        public Student Student { get; set; } = null!;
        public Route Route { get; set; } = null!;
    }
}
