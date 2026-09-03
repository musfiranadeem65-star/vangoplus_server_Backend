namespace vangoplus_server.Application.DTOs
{
    public class RouteStopInputDto
    {
        public string StopName { get; set; } = null!;
        public TimeSpan? ArrivalTime { get; set; }
        public int OrderIndex { get; set; }
    }
}
