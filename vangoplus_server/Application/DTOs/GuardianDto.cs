namespace vangoplus_server.Application.DTOs
{
    public class GuardianDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string Name { get; set; } = null!;
        public string Relation { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Status { get; set; }
        public string? Note { get; set; }
        public string? IdentityDocumentPath { get; set; }
    }
}
