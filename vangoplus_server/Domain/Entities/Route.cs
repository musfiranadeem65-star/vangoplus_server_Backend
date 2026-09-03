namespace vangoplus_server.Domain.Entities
{
    public class Route
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Status { get; set; } = null!;
        public int DriverId { get; set; }
        public string? Description { get; set; }

        // Navigation
        public Driver Driver { get; set; } = null!;
        public ICollection<RouteStop> RouteStops { get; set; } = new List<RouteStop>();
    }
}
