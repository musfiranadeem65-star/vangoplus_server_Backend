using vangoplus_server.Application.DTOs;

namespace vangoplus_server.Application.Interfaces
{
    public interface ISchoolSettingService
    {
        Task<SchoolSettingDto> GetAsync();
        Task<SchoolSettingDto> UpdateAsync(UpdateSchoolSettingDto dto);
    }
}
