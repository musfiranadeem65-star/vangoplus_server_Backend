using Microsoft.EntityFrameworkCore;
using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;
using vangoplus_server.Domain.Entities;
using vangoplus_server.Infrastructure.Data;

namespace vangoplus_server.Application.Services
{
    public class TripRecordService : ITripRecordService
    {
        private readonly AppDbContext _db;

        public TripRecordService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<TripRecordDto> RecordAsync(TripRecordInputDto dto, int recordedByUserId)
        {
            var occurredAt = dto.OccurredAt ?? DateTime.UtcNow;

            var entity = new TripRecord
            {
                StudentId = dto.StudentId,
                RouteId = dto.RouteId,
                Kind = dto.Kind,
                OccurredAt = occurredAt,
                OnDate = SchoolDayOf(occurredAt),
                RecordedByUserId = recordedByUserId,
                Note = dto.Note
            };

            _db.TripRecords.Add(entity);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (IsDuplicate(ex))
            {
                // The unique index refused a second row for this child, day and
                // kind. Two taps on one button is the common cause, and the
                // honest answer is the record that already exists, not an error
                // and not a second row.
                _db.Entry(entity).State = EntityState.Detached;

                var existing = await _db.TripRecords
                    .AsNoTracking()
                    .FirstAsync(t =>
                        t.StudentId == entity.StudentId &&
                        t.OnDate == entity.OnDate &&
                        t.Kind == entity.Kind);

                return await ToDtoAsync(existing);
            }

            return await ToDtoAsync(entity);
        }

        public async Task<IEnumerable<StudentDayStatusDto>> GetRouteDayAsync(int routeId, DateOnly onDate)
        {
            // Every child assigned to this route, whether or not anything has
            // been recorded yet: an empty row is the one the operator still
            // has to mark.
            var assigned = await _db.StudentRouteAssignments
                .AsNoTracking()
                .Where(a => a.RouteId == routeId)
                .Include(a => a.Student)
                .Include(a => a.Route)
                .ToListAsync();

            var studentIds = assigned.Select(a => a.StudentId).ToList();

            var records = await _db.TripRecords
                .AsNoTracking()
                .Where(t => t.RouteId == routeId
                            && t.OnDate == onDate
                            && studentIds.Contains(t.StudentId))
                .ToListAsync();

            return assigned
                .Select(a => BuildStatus(
                    a.StudentId,
                    a.Student.Name,
                    a.RouteId,
                    a.Route.Name,
                    onDate,
                    records))
                .OrderBy(s => s.StudentName)
                .ToList();
        }

        public async Task<IEnumerable<StudentDayStatusDto>> GetForParentAsync(int parentUserId, int days)
        {
            var from = SchoolDayOf(DateTime.UtcNow).AddDays(-Math.Max(days - 1, 0));

            var children = await _db.Students
                .AsNoTracking()
                .Where(s => s.ParentUserId == parentUserId)
                .ToListAsync();

            if (children.Count == 0)
                return Array.Empty<StudentDayStatusDto>();

            var childIds = children.Select(c => c.Id).ToList();

            var records = await _db.TripRecords
                .AsNoTracking()
                .Where(t => childIds.Contains(t.StudentId) && t.OnDate >= from)
                .Include(t => t.Route)
                .ToListAsync();

            // A day is listed only when something was recorded on it, so a
            // parent does not scroll past a week of empty rows.
            return records
                .GroupBy(t => new { t.StudentId, t.OnDate, t.RouteId })
                .Select(g =>
                {
                    var child = children.First(c => c.Id == g.Key.StudentId);

                    return BuildStatus(
                        g.Key.StudentId,
                        child.Name,
                        g.Key.RouteId,
                        g.First().Route.Name,
                        g.Key.OnDate,
                        g.ToList());
                })
                .OrderByDescending(s => s.OnDate)
                .ThenBy(s => s.StudentName)
                .ToList();
        }

        public async Task<bool> StudentBelongsToParentAsync(int studentId, int parentUserId) =>
            await _db.Students
                .AsNoTracking()
                .AnyAsync(s => s.Id == studentId && s.ParentUserId == parentUserId);

        /// <summary>
        /// Which school day a moment belongs to. Timestamps are stored in UTC
        /// and Pakistan is UTC+5, so a 7:40am pickup is 2:40am UTC and would
        /// otherwise be filed against the day before.
        /// </summary>
        private static DateOnly SchoolDayOf(DateTime utc) =>
            DateOnly.FromDateTime(utc.AddHours(5));

        private static StudentDayStatusDto BuildStatus(
            int studentId,
            string studentName,
            int routeId,
            string routeName,
            DateOnly onDate,
            IReadOnlyCollection<TripRecord> records) =>
            new()
            {
                StudentId = studentId,
                StudentName = studentName,
                RouteId = routeId,
                RouteName = routeName,
                OnDate = onDate,
                BoardedAt = records
                    .FirstOrDefault(t => t.StudentId == studentId
                                         && t.OnDate == onDate
                                         && t.Kind == TripRecordKind.Boarded)?.OccurredAt,
                DroppedAt = records
                    .FirstOrDefault(t => t.StudentId == studentId
                                         && t.OnDate == onDate
                                         && t.Kind == TripRecordKind.Dropped)?.OccurredAt
            };

        private async Task<TripRecordDto> ToDtoAsync(TripRecord entity)
        {
            var student = await _db.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == entity.StudentId);

            var route = await _db.Routes
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == entity.RouteId);

            return new TripRecordDto
            {
                Id = entity.Id,
                StudentId = entity.StudentId,
                StudentName = student?.Name ?? string.Empty,
                RouteId = entity.RouteId,
                RouteName = route?.Name ?? string.Empty,
                Kind = entity.Kind,
                OccurredAt = entity.OccurredAt,
                OnDate = entity.OnDate,
                Note = entity.Note
            };
        }

        private static bool IsDuplicate(DbUpdateException ex) =>
            ex.InnerException is Npgsql.PostgresException pg &&
            pg.SqlState == "23505";
    }
}
