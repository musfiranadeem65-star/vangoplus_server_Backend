using Microsoft.EntityFrameworkCore;
using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;
using vangoplus_server.Domain.Entities;
using vangoplus_server.Infrastructure.Data;

namespace vangoplus_server.Application.Services
{
    public class RouteService : IRouteService
    {
        private readonly AppDbContext _db;

        public RouteService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<RouteDto> CreateAsync(RouteDto dto)
        {
            var entity = new Domain.Entities.Route
            {
                Name = dto.Name,
                Status = dto.Status,
                DriverId = dto.DriverId,
                Description = dto.Description
            };

            _db.Routes.Add(entity);
            await _db.SaveChangesAsync();

            // Create route stops if provided
            if (dto.RouteStops != null && dto.RouteStops.Count > 0)
            {
                var stops = dto.RouteStops.Select(rs => new RouteStop
                {
                    RouteId = entity.Id,
                    StopName = rs.StopName,
                    ArrivalTime = rs.ArrivalTime,
                    OrderIndex = rs.OrderIndex
                }).ToList();

                _db.RouteStops.AddRange(stops);
                await _db.SaveChangesAsync();
            }

            dto.Id = entity.Id;
            return dto;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.Routes.FindAsync(id);
            if (entity == null) return;
            _db.Routes.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<RouteDto>> GetAllAsync()
        {
            return await _db.Routes.AsNoTracking()
                .Select(r => new RouteDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Status = r.Status,
                    DriverId = r.DriverId,
                    Description = r.Description,
                    RouteStops = r.RouteStops.Select(rs => new RouteStopInputDto
                    {
                        StopName = rs.StopName,
                        ArrivalTime = rs.ArrivalTime,
                        OrderIndex = rs.OrderIndex
                    }).OrderBy(rs => rs.OrderIndex).ToList()
                })
                .ToListAsync();
        }

        public async Task<RouteDto?> GetByIdAsync(int id)
        {
            var entity = await _db.Routes.AsNoTracking()
                .Include(r => r.RouteStops)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (entity == null) return null;

            return new RouteDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Status = entity.Status,
                DriverId = entity.DriverId,
                Description = entity.Description,
                RouteStops = entity.RouteStops.Select(rs => new RouteStopInputDto
                {
                    StopName = rs.StopName,
                    ArrivalTime = rs.ArrivalTime,
                    OrderIndex = rs.OrderIndex
                }).OrderBy(rs => rs.OrderIndex).ToList()
            };
        }

        public async Task UpdateAsync(int id, RouteDto dto)
        {
            var existing = await _db.Routes.FindAsync(id);
            if (existing == null) return;

            existing.Name = dto.Name;
            existing.Status = dto.Status;
            existing.DriverId = dto.DriverId;
            existing.Description = dto.Description;
            await _db.SaveChangesAsync();

            // Handle route stops update: delete old ones and create new ones
            if (dto.RouteStops != null)
            {
                // Delete existing route stops for this route
                var existingStops = await _db.RouteStops.Where(rs => rs.RouteId == id).ToListAsync();
                _db.RouteStops.RemoveRange(existingStops);
                await _db.SaveChangesAsync();

                // Create new route stops
                if (dto.RouteStops.Count > 0)
                {
                    var stops = dto.RouteStops.Select(rs => new RouteStop
                    {
                        RouteId = id,
                        StopName = rs.StopName,
                        ArrivalTime = rs.ArrivalTime,
                        OrderIndex = rs.OrderIndex
                    }).ToList();

                    _db.RouteStops.AddRange(stops);
                    await _db.SaveChangesAsync();
                }
            }
        }

        public async Task UpdateStatusAsync(int routeId, string status)
        {
            var existing = await _db.Routes.FindAsync(routeId);
            if (existing == null) return;
            existing.Status = status;
            await _db.SaveChangesAsync();
        }
    }
}
