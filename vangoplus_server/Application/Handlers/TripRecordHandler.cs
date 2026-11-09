using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;
using vangoplus_server.Domain.Entities;

namespace vangoplus_server.Application.Handlers
{
    public class TripRecordHandler : ITripRecordHandler
    {
        private readonly ITripRecordService _service;
        private readonly IStudentService _studentService;
        private readonly IRouteService _routeService;

        public TripRecordHandler(
            ITripRecordService service,
            IStudentService studentService,
            IRouteService routeService)
        {
            _service = service;
            _studentService = studentService;
            _routeService = routeService;
        }

        public async Task<TripRecordDto> RecordAsync(TripRecordInputDto dto, int recordedByUserId)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
            if (dto.StudentId <= 0)
                throw new ArgumentException("StudentId must be greater than zero.", nameof(dto.StudentId));
            if (dto.RouteId <= 0)
                throw new ArgumentException("RouteId must be greater than zero.", nameof(dto.RouteId));
            if (!TripRecordKind.IsValid(dto.Kind))
                throw new ArgumentException(
                    $"Kind must be {TripRecordKind.Boarded} or {TripRecordKind.Dropped}.",
                    nameof(dto.Kind));

            // A time in the future is a typo, and it would file the record
            // against a school day that has not happened.
            if (dto.OccurredAt.HasValue && dto.OccurredAt.Value > DateTime.UtcNow.AddMinutes(5))
                throw new ArgumentException("OccurredAt cannot be in the future.", nameof(dto.OccurredAt));

            var student = await _studentService.GetByIdAsync(dto.StudentId);
            if (student == null)
                throw new InvalidOperationException("Student not found.");

            var route = await _routeService.GetByIdAsync(dto.RouteId);
            if (route == null)
                throw new InvalidOperationException("Route not found.");

            return await _service.RecordAsync(dto, recordedByUserId);
        }

        public async Task<IEnumerable<StudentDayStatusDto>> GetRouteDayAsync(int routeId, DateOnly? onDate)
        {
            if (routeId <= 0)
                throw new ArgumentException("RouteId must be greater than zero.", nameof(routeId));

            var route = await _routeService.GetByIdAsync(routeId);
            if (route == null)
                throw new InvalidOperationException("Route not found.");

            // Pakistan is UTC+5, so "today" for a school is not today in UTC
            // after 7pm local time.
            var day = onDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddHours(5));

            return await _service.GetRouteDayAsync(routeId, day);
        }

        public async Task<IEnumerable<StudentDayStatusDto>> GetForParentAsync(int parentUserId, int days)
        {
            if (parentUserId <= 0)
                throw new ArgumentException("A signed-in parent is required.", nameof(parentUserId));

            // A window, so a parent cannot ask for the whole history in one
            // request as the family's record grows.
            var window = Math.Clamp(days, 1, 30);

            return await _service.GetForParentAsync(parentUserId, window);
        }
    }
}
