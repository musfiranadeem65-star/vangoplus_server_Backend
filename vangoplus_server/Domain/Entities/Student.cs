namespace vangoplus_server.Domain.Entities
{
    public class Student
    {
        public int Id { get; set; }
        public int ParentUserId { get; set; }
        public string Name { get; set; } = null!;
        public string Grade { get; set; } = null!;
        public string? Section { get; set; }
        public string? Status { get; set; }

        // Navigation
        public User? ParentUser { get; set; }
        public ICollection<Alert> Alerts { get; set; } = new List<Alert>();
    }
}
