namespace vangoplus_server.Application.DTOs
{
    public class StudentDto
    {
        public int Id { get; set; }
        public int ParentUserId { get; set; }
        public string Name { get; set; } = null!;
        public string Grade { get; set; } = null!;
        public string? Section { get; set; }
        public string? Status { get; set; }
    }
}
