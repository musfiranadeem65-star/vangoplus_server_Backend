using vangoplus_server.Application.DTOs;

namespace vangoplus_server.Application.Interfaces
{
    public interface IGuardianService
    {
        Task<GuardianDto> CreateAsync(GuardianDto dto);
        Task<IEnumerable<GuardianDto>> GetAllAsync();
        Task<GuardianDto?> GetByIdAsync(int id);
        Task UpdateAsync(int id, GuardianDto dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<GuardianDto>> GetByStudentIdAsync(int studentId);
        Task LinkToStudentAsync(int studentId, int guardianId, StudentGuardianDto linkDto);
        Task UpdateStatusAsync(int guardianId, string status);
    }
}
