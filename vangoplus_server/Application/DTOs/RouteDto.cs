namespace vangoplus_server.Application.DTOs
{
    public class RouteDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Status { get; set; } = null!;
        public int DriverId { get; set; }
        public string? Description { get; set; }
        public List<RouteStopInputDto> RouteStops { get; set; } = new();
    }
}
