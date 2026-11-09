using Microsoft.EntityFrameworkCore;
using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;
using vangoplus_server.Infrastructure.Data;

namespace vangoplus_server.Application.Services
{
    public class StudentScheduleService : IStudentScheduleService
    {
        private readonly AppDbContext _db;

        public StudentScheduleService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<StudentScheduleDto?> GetAsync(int studentId)
        {
            var student = await _db.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null) return null;

            var schedule = new StudentScheduleDto
            {
                StudentId = student.Id,
                StudentName = student.Name
            };

            // The most recent assignment, so re-assigning a child to another
            // route shows the new one rather than whichever row came back
            // first.
            var assignment = await _db.StudentRouteAssignments
                .AsNoTracking()
                .Where(a => a.StudentId == studentId)
                .OrderByDescending(a => a.AssignedAt)
                .ThenByDescending(a => a.Id)
                .FirstOrDefaultAsync();

            if (assignment == null) return schedule;

            schedule.RouteId = assignment.RouteId;
            schedule.PickupTime = assignment.PickupTime;
            schedule.DropoffTime = assignment.DropoffTime;

            var route = await _db.Routes
                .AsNoTracking()
                .Include(r => r.Driver)
                .Include(r => r.RouteStops)
                .FirstOrDefaultAsync(r => r.Id == assignment.RouteId);

            if (route == null) return schedule;

            schedule.RouteName = route.Name;
            schedule.DriverId = route.DriverId;
            schedule.DriverName = route.Driver?.Name;

            schedule.RouteStops = route.RouteStops
                .OrderBy(rs => rs.OrderIndex)
                .Select(rs => new RouteStopDto
                {
                    Id = rs.Id,
                    RouteId = rs.RouteId,
                    StopName = rs.StopName,
                    ArrivalTime = rs.ArrivalTime,
                    OrderIndex = rs.OrderIndex,
                    Latitude = rs.Latitude,
                    Longitude = rs.Longitude
                })
                .ToList();

            return schedule;
        }

        public async Task<bool> BelongsToParentAsync(int studentId, int parentUserId) =>
            await _db.Students
                .AsNoTracking()
                .AnyAsync(s => s.Id == studentId && s.ParentUserId == parentUserId);
    }
}
