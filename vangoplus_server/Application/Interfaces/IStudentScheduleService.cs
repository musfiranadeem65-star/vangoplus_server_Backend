using vangoplus_server.Application.DTOs;

namespace vangoplus_server.Application.Interfaces
{
    public interface IStudentScheduleService
    {
        /// <summary>
        /// The child's route and its stops, or null when there is no such
        /// child. A child with no route assignment comes back with the name
        /// filled in and no route, which is what the screen shows as
        /// "not assigned yet".
        /// </summary>
        Task<StudentScheduleDto?> GetAsync(int studentId);

        Task<bool> BelongsToParentAsync(int studentId, int parentUserId);
    }
}
