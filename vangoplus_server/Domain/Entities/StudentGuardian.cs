namespace vangoplus_server.Domain.Entities;

public class StudentGuardian
{
    public int StudentId { get; set; }
    public int GuardianId { get; set; }
    public bool IsPrimary { get; set; }
    public string? Status { get; set; }

    // Navigation properties
    public Student? Student { get; set; } 
    public Guardian? Guardian { get; set; } 
}
