namespace vangoplus_server.Application.DTOs
{
    public class LoginResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Role { get; set; }
        public string? Phone { get; set; }
        public string? City { get; set; }
        public string? Status { get; set; }
    }
}
