using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;
using vangoplus_server.Application.Services;

namespace vangoplus_server.Application.Handlers
{
    public class GuardianHandler : IGuardianHandler
    {
        private readonly IGuardianService _service;
        private readonly IUserService _userService;
        private readonly IStudentService _studentService;

        public GuardianHandler(IGuardianService service, IUserService userService, IStudentService studentService)
        {
            _service = service;
            _userService = userService;
            _studentService = studentService;
        }

        public async Task<GuardianDto> CreateAsync(GuardianDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Name is required.", nameof(dto.Name));
            if (string.IsNullOrWhiteSpace(dto.Relation))
                throw new ArgumentException("Relation is required.", nameof(dto.Relation));
            if (dto.UserId <= 0)
                throw new ArgumentException("UserId is required.", nameof(dto.UserId));
            if (string.IsNullOrWhiteSpace(dto.Status))
                throw new ArgumentException("Status is required.", nameof(dto.Status));

            // Ensure user exists
            var user = await _userService.GetByIdAsync(dto.UserId);
            if (user == null)
                throw new InvalidOperationException("User not found.");

            return await _service.CreateAsync(dto);
        }

        public async Task DeleteAsync(int id)
        {
            await _service.DeleteAsync(id);
        }
       

        public async Task<IEnumerable<GuardianDto>> GetAllAsync()
        {
            return await _service.GetAllAsync();
        }
       
        public async Task<GuardianDto?> GetByIdAsync(int id)
        {
            return await _service.GetByIdAsync(id);
        }

        public async Task UpdateAsync(int id, GuardianDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Name is required.", nameof(dto.Name));
            if (string.IsNullOrWhiteSpace(dto.Relation))
                throw new ArgumentException("Relation is required.", nameof(dto.Relation));
            if (dto.UserId <= 0)
                throw new ArgumentException("UserId is required.", nameof(dto.UserId));
            if (string.IsNullOrWhiteSpace(dto.Status))
                throw new ArgumentException("Status is required.", nameof(dto.Status));

            var user = await _userService.GetByIdAsync(dto.UserId);
            if (user == null)
                throw new InvalidOperationException("User not found.");

            await _service.UpdateAsync(id, dto);
        }

        public async Task<IEnumerable<GuardianDto>> GetByStudentIdAsync(int studentId)
        {
            return await _service.GetByStudentIdAsync(studentId);
        }

        public async Task LinkToStudentAsync(int studentId, int guardianId, StudentGuardianDto linkDto)
        {
            // Validate student exists
            var student = await _studentService.GetByIdAsync(studentId);
            if (student == null)
                throw new InvalidOperationException("Student not found.");

            // Validate guardian exists
            var guardian = await _service.GetByIdAsync(guardianId);
            if (guardian == null)
                throw new InvalidOperationException("Guardian not found.");

            await _service.LinkToStudentAsync(studentId, guardianId, linkDto);
        }

        public async Task UpdateStatusAsync(int guardianId, string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("Status is required.", nameof(status));
            var guardian = await _service.GetByIdAsync(guardianId);
            if (guardian == null)
                throw new InvalidOperationException("Guardian not found.");
            await _service.UpdateStatusAsync(guardianId, status);
        }
    }
}
