using vangoplus_server.Application.DTOs;

namespace vangoplus_server.Application.Interfaces
{
    public interface IStudentRouteAssignmentService
    {
        Task<StudentRouteAssignmentDto> CreateAsync(StudentRouteAssignmentDto dto);
        Task<IEnumerable<StudentRouteAssignmentDto>> GetAllAsync();
        Task<StudentRouteAssignmentDto?> GetByIdAsync(int id);
        Task UpdateAsync(int id, StudentRouteAssignmentDto dto);
        Task UpdateStatusAsync(int id, string status);
        Task DeleteAsync(int id);
    }
}
