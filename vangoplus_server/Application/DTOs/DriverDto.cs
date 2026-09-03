namespace vangoplus_server.Application.DTOs
{
    public class DriverDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string LicenseNo { get; set; } = null!;
        public string? Status { get; set; }
    }
}
