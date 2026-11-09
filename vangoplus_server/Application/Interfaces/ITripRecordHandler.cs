using vangoplus_server.Application.DTOs;

namespace vangoplus_server.Application.Interfaces
{
    public interface ITripRecordHandler
    {
        Task<TripRecordDto> RecordAsync(TripRecordInputDto dto, int recordedByUserId);
        Task<IEnumerable<StudentDayStatusDto>> GetRouteDayAsync(int routeId, DateOnly? onDate);
        Task<IEnumerable<StudentDayStatusDto>> GetForParentAsync(int parentUserId, int days);
    }
}
