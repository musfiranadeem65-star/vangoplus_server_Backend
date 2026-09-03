using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;

namespace vangoplus_server.Application.Handlers
{
    public class StudentHandler : IStudentHandler
    {
        private readonly IStudentService _service;
        private readonly IUserService _userService;

        public StudentHandler(IStudentService service, IUserService userService)
        {
            _service = service;
            _userService = userService;
        }

        public async Task<StudentDto> CreateAsync(StudentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Name is required.", nameof(dto.Name));
            if (string.IsNullOrWhiteSpace(dto.Grade))
                throw new ArgumentException("Grade is required.", nameof(dto.Grade));
            if (dto.ParentUserId <= 0)
                throw new ArgumentException("ParentUserId is required.", nameof(dto.ParentUserId));
            if (string.IsNullOrWhiteSpace(dto.Status))
                throw new ArgumentException("Status is required.", nameof(dto.Status));

            // Ensure parent exists
            var parent = await _userService.GetByIdAsync(dto.ParentUserId);
            if (parent == null)
                throw new InvalidOperationException("Parent user not found.");

            return await _service.CreateAsync(dto);
        }

        public async Task DeleteAsync(int id)
        {
            await _service.DeleteAsync(id);
        }

        public async Task<IEnumerable<StudentDto>> GetAllAsync()
        {
            return await _service.GetAllAsync();
        }

        public async Task<StudentDto?> GetByIdAsync(int id)
        {
            return await _service.GetByIdAsync(id);
        }

        public async Task UpdateAsync(int id, StudentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Name is required.", nameof(dto.Name));
            if (string.IsNullOrWhiteSpace(dto.Grade))
                throw new ArgumentException("Grade is required.", nameof(dto.Grade));
            if (dto.ParentUserId <= 0)
                throw new ArgumentException("ParentUserId is required.", nameof(dto.ParentUserId));
            if (string.IsNullOrWhiteSpace(dto.Status))
                throw new ArgumentException("Status is required.", nameof(dto.Status));

            var parent = await _userService.GetByIdAsync(dto.ParentUserId);
            if (parent == null)
                throw new InvalidOperationException("Parent user not found.");

            await _service.UpdateAsync(id, dto);
        }

        public async Task<IEnumerable<StudentDto>> GetByParentIdAsync(int parentId)
        {
            return await _service.GetByParentIdAsync(parentId);
        }
    }
}
