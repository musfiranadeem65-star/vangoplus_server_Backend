using vangoplus_server.Application.DTOs;

namespace vangoplus_server.Application.Interfaces
{
    public interface IAlertHandler
    {
        Task<AlertDto> CreateAsync(AlertDto dto);
        Task<IEnumerable<AlertDto>> GetByStudentIdAsync(int studentId);
        Task<AlertDto?> GetByIdAsync(int id);
        Task UpdateReadStatusAsync(int alertId);
    }
}
