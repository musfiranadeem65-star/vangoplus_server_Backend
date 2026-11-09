using vangoplus_server.Application.DTOs;

namespace vangoplus_server.Application.Interfaces
{
    public interface ITripRecordService
    {
        Task<TripRecordDto> RecordAsync(TripRecordInputDto dto, int recordedByUserId);

        /// <summary>The roster for one route on one day, every assigned child.</summary>
        Task<IEnumerable<StudentDayStatusDto>> GetRouteDayAsync(int routeId, DateOnly onDate);

        /// <summary>
        /// One parent's own children, most recent day first. Scoped by parent
        /// id, so a parent cannot read another family's rows.
        /// </summary>
        Task<IEnumerable<StudentDayStatusDto>> GetForParentAsync(int parentUserId, int days);

        Task<bool> StudentBelongsToParentAsync(int studentId, int parentUserId);
    }
}
