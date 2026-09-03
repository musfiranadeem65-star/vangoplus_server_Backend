namespace vangoplus_server.Application.DTOs
{
    public class RouteStopDto
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public string StopName { get; set; } = null!;
        public TimeSpan? ArrivalTime { get; set; }
        public int OrderIndex { get; set; }
    }
}
