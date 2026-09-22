namespace vangoplus_server.Domain.Entities
{
    public class SchoolSetting
    {
        public int Id { get; set; }
        public string SchoolName { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public string? SchoolAddress { get; set; }
        public decimal MonthlyAmount { get; set; }
        public string? SenderEmail { get; set; }
    }
}
