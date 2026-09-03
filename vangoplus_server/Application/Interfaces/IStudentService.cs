using vangoplus_server.Application.DTOs;

namespace vangoplus_server.Application.Interfaces
{
    public interface IStudentService
    {
        Task<StudentDto> CreateAsync(StudentDto dto);
        Task<IEnumerable<StudentDto>> GetAllAsync();
        Task<StudentDto?> GetByIdAsync(int id);
        Task UpdateAsync(int id, StudentDto dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<StudentDto>> GetByParentIdAsync(int parentId);
    }
}
