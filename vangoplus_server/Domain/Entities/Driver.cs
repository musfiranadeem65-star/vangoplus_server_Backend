namespace vangoplus_server.Domain.Entities
{
    public class Driver
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string LicenseNo { get; set; } = null!;
        public string? Status { get; set; }

        // Navigation
        public ICollection<Route> Routes { get; set; } = new List<Route>();
    }
}

