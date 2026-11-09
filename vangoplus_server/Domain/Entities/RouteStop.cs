namespace vangoplus_server.Domain.Entities
{
    public class RouteStop
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public string StopName { get; set; } = null!;
        public TimeSpan? ArrivalTime { get; set; }
        public int OrderIndex { get; set; }

        // Where the stop is, so the parent portal can draw it on a map.
        // Nullable because every stop recorded before this existed has no
        // coordinates, and a stop with only a name is still a usable stop.
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // Navigation
        public Route Route { get; set; } = null!;
    }
}
