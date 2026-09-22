using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;

namespace vangoplus_server.Application.Handlers
{
    public class SchoolSettingHandler : ISchoolSettingHandler
    {
        private readonly ISchoolSettingService _service;

        public SchoolSettingHandler(ISchoolSettingService service)
        {
            _service = service;
        }

        public async Task<SchoolSettingDto> GetAsync()
        {
            return await _service.GetAsync();
        }

        public async Task<SchoolSettingDto> UpdateAsync(UpdateSchoolSettingDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.SchoolName))
                throw new ArgumentException("SchoolName is required.", nameof(dto.SchoolName));
            if (dto.MonthlyAmount < 0)
                throw new ArgumentException("MonthlyAmount must be zero or greater.", nameof(dto.MonthlyAmount));

            return await _service.UpdateAsync(dto);
        }
    }
}
