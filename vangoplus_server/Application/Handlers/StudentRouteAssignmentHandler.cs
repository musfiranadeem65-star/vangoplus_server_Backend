using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;

namespace vangoplus_server.Application.Handlers
{
    public class StudentRouteAssignmentHandler : IStudentRouteAssignmentHandler
    {
        private readonly IStudentRouteAssignmentService _service;
        private readonly IStudentService _studentService;
        private readonly IRouteService _routeService;

        public StudentRouteAssignmentHandler(
            IStudentRouteAssignmentService service,
            IStudentService studentService,
            IRouteService routeService)
        {
            _service = service;
            _studentService = studentService;
            _routeService = routeService;
        }

        public async Task<StudentRouteAssignmentDto> CreateAsync(StudentRouteAssignmentDto dto)
        {
            if (dto.StudentId <= 0)
                throw new ArgumentException("StudentId must be greater than zero.", nameof(dto.StudentId));
            if (dto.RouteId <= 0)
                throw new ArgumentException("RouteId must be greater than zero.", nameof(dto.RouteId));
            if (string.IsNullOrWhiteSpace(dto.Status))
                throw new ArgumentException("Status is required.", nameof(dto.Status));

            // Check if Student exists
            var student = await _studentService.GetByIdAsync(dto.StudentId);
            if (student == null)
                throw new InvalidOperationException("Student not found.");

            // Check if Route exists
            var route = await _routeService.GetByIdAsync(dto.RouteId);
            if (route == null)
                throw new InvalidOperationException("Route not found.");

            dto.AssignedAt = DateTime.UtcNow;
            return await _service.CreateAsync(dto);
        }

        public async Task<IEnumerable<StudentRouteAssignmentDto>> GetAllAsync()
        {
            return await _service.GetAllAsync();
        }

        public async Task<StudentRouteAssignmentDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than zero.", nameof(id));

            return await _service.GetByIdAsync(id);
        }

        public async Task UpdateAsync(int id, StudentRouteAssignmentDto dto)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than zero.", nameof(id));
            if (dto.StudentId <= 0)
                throw new ArgumentException("StudentId must be greater than zero.", nameof(dto.StudentId));
            if (dto.RouteId <= 0)
                throw new ArgumentException("RouteId must be greater than zero.", nameof(dto.RouteId));
            if (string.IsNullOrWhiteSpace(dto.Status))
                throw new ArgumentException("Status is required.", nameof(dto.Status));

            // Check if StudentRouteAssignment exists
            var existing = await _service.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException($"StudentRouteAssignment with Id {id} not found.");

            // Check if Student exists
            var student = await _studentService.GetByIdAsync(dto.StudentId);
            if (student == null)
                throw new InvalidOperationException("Student not found.");

            // Check if Route exists
            var route = await _routeService.GetByIdAsync(dto.RouteId);
            if (route == null)
                throw new InvalidOperationException("Route not found.");

            await _service.UpdateAsync(id, dto);
        }

        public async Task UpdateStatusAsync(int id, string status)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than zero.", nameof(id));
            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("Status is required.", nameof(status));

            // Check if StudentRouteAssignment exists
            var existing = await _service.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException($"StudentRouteAssignment with Id {id} not found.");

            await _service.UpdateStatusAsync(id, status);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than zero.", nameof(id));

            // Check if StudentRouteAssignment exists
            var existing = await _service.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException($"StudentRouteAssignment with Id {id} not found.");

            await _service.DeleteAsync(id);
        }
    }
}
