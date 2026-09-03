using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;

namespace vangoplus_server.Application.Handlers
{
    public class AlertHandler : IAlertHandler
    {
        private readonly IAlertService _service;
        private readonly IStudentService _studentService;

        public AlertHandler(IAlertService service, IStudentService studentService)
        {
            _service = service;
            _studentService = studentService;
        }

        public async Task<AlertDto> CreateAsync(AlertDto dto)
        {
            if (dto.StudentId <= 0)
                throw new ArgumentException("StudentId must be greater than zero.", nameof(dto.StudentId));
            if (string.IsNullOrWhiteSpace(dto.Type))
                throw new ArgumentException("Type is required.", nameof(dto.Type));
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("Title is required.", nameof(dto.Title));
            if (string.IsNullOrWhiteSpace(dto.Message))
                throw new ArgumentException("Message is required.", nameof(dto.Message));

            // Check if Student exists
            var student = await _studentService.GetByIdAsync(dto.StudentId);
            if (student == null)
                throw new InvalidOperationException("Student not found.");

            return await _service.CreateAsync(dto);
        }

        public async Task<IEnumerable<AlertDto>> GetByStudentIdAsync(int studentId)
        {
            if (studentId <= 0)
                throw new ArgumentException("StudentId must be greater than zero.", nameof(studentId));

            return await _service.GetByStudentIdAsync(studentId);
        }

        public async Task<AlertDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than zero.", nameof(id));

            return await _service.GetByIdAsync(id);
        }

        public async Task UpdateReadStatusAsync(int alertId)
        {
            if (alertId <= 0)
                throw new ArgumentException("AlertId must be greater than zero.", nameof(alertId));

            await _service.UpdateReadStatusAsync(alertId);
        }
    }
}
