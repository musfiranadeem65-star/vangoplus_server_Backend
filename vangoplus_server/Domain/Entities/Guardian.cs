namespace vangoplus_server.Domain.Entities
{
    public class Guardian
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public string Relation { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Status { get; set; }
        public string? Note { get; set; }
        public string? IdentityDocumentPath { get; set; }

        // Navigation
        public User? User { get; set; }
        public ICollection<StudentGuardian> StudentGuardians { get; set; } = new List<StudentGuardian>();
    }
}

