namespace vangoplus_server.Domain.Entities
{
    /// <summary>
    /// One thing that happened to one child on one day: they boarded the van,
    /// or they were dropped off.
    ///
    /// A row per event rather than two columns on the assignment, because a
    /// child can be picked up and dropped every school day, and a single pair
    /// of columns can only ever describe the most recent day.
    /// </summary>
    public class TripRecord
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public int RouteId { get; set; }

        /// <summary>"Boarded" or "Dropped" — see TripRecordKind.</summary>
        public string Kind { get; set; } = null!;

        /// <summary>
        /// When it happened, in UTC. The school day it belongs to is
        /// <see cref="OnDate"/>, which is kept separately so a late evening
        /// drop is not filed against the next day.
        /// </summary>
        public DateTime OccurredAt { get; set; }

        public DateOnly OnDate { get; set; }

        /// <summary>Who recorded it, so an entry can be traced to a person.</summary>
        public int RecordedByUserId { get; set; }

        public string? Note { get; set; }

        // Navigation
        public Student Student { get; set; } = null!;
        public Route Route { get; set; } = null!;
    }

    public static class TripRecordKind
    {
        public const string Boarded = "Boarded";
        public const string Dropped = "Dropped";

        public static bool IsValid(string? kind) =>
            kind == Boarded || kind == Dropped;
    }
}
