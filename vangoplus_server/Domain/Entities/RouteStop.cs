namespace vangoplus_server.Domain.Entities
{
    public class RouteStop
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public string StopName { get; set; } = null!;
        public TimeSpan? ArrivalTime { get; set; }
        public int OrderIndex { get; set; }

        // Navigation
        public Route Route { get; set; } = null!;
    }
}
