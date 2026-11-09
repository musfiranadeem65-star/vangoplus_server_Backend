namespace vangoplus_server.Application.DTOs
{
    public class TripRecordInputDto
    {
        public int StudentId { get; set; }
        public int RouteId { get; set; }

        /// <summary>"Boarded" or "Dropped".</summary>
        public string Kind { get; set; } = null!;

        /// <summary>
        /// When it happened. Left empty by the button on the screen, which
        /// means now; set only when somebody is entering a time after the fact.
        /// </summary>
        public DateTime? OccurredAt { get; set; }

        public string? Note { get; set; }
    }
}
