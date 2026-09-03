namespace vangoplus_server.Application.DTOs
{
    public class StudentGuardianDto
    {
        public int StudentId { get; set; }
        public int GuardianId { get; set; }
        public bool IsPrimary { get; set; }
        public string Status { get; set; } = null!;
    }
}
