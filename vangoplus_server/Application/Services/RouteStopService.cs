using Microsoft.EntityFrameworkCore;
using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;
using vangoplus_server.Domain.Entities;
using vangoplus_server.Infrastructure.Data;

namespace vangoplus_server.Application.Services
{
    public class RouteStopService : IRouteStopService
    {
        private readonly AppDbContext _db;

        public RouteStopService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<RouteStopDto> CreateAsync(RouteStopDto dto)
        {
            var entity = new RouteStop
            {
                RouteId = dto.RouteId,
                StopName = dto.StopName,
                ArrivalTime = dto.ArrivalTime,
                OrderIndex = dto.OrderIndex
            };

            _db.RouteStops.Add(entity);
            await _db.SaveChangesAsync();

            dto.Id = entity.Id;
            return dto;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.RouteStops.FindAsync(id);
            if (entity == null) return;
            _db.RouteStops.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<RouteStopDto?> GetByIdAsync(int id)
        {
            var entity = await _db.RouteStops.FindAsync(id);
            if (entity == null) return null;
            return new RouteStopDto
            {
                Id = entity.Id,
                RouteId = entity.RouteId,
                StopName = entity.StopName,
                ArrivalTime = entity.ArrivalTime,
                OrderIndex = entity.OrderIndex
            };
        }

        public async Task<IEnumerable<RouteStopDto>> GetByRouteIdAsync(int routeId)
        {
            return await _db.RouteStops.AsNoTracking()
                .Where(rs => rs.RouteId == routeId)
                .OrderBy(rs => rs.OrderIndex)
                .Select(rs => new RouteStopDto
                {
                    Id = rs.Id,
                    RouteId = rs.RouteId,
                    StopName = rs.StopName,
                    ArrivalTime = rs.ArrivalTime,
                    OrderIndex = rs.OrderIndex
                })
                .ToListAsync();
        }

        public async Task UpdateAsync(int id, RouteStopDto dto)
        {
            var existing = await _db.RouteStops.FindAsync(id);
            if (existing == null) return;
            existing.RouteId = dto.RouteId;
            existing.StopName = dto.StopName;
            existing.ArrivalTime = dto.ArrivalTime;
            existing.OrderIndex = dto.OrderIndex;
            await _db.SaveChangesAsync();
        }
    }
}
