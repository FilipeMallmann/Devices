using Devices.Domain;
using Devices.Domain.Interfaces;
using Devices.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Devices.Infrastructure.Repositorys
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly DevicesDbContext _db;

        public DeviceRepository(DevicesDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(DeviceModel device, CancellationToken ct = default)
        {
            await _db.Devices.AddAsync(device, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var entity = await _db.Devices.FindAsync(id, ct);
            if (entity is null) return;
            _db.Devices.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<DeviceModel?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var devices = _db.Devices.ToList();

            return await _db.Devices.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id, ct);
        }

        public async Task<IEnumerable<DeviceModel>> ListAsync(CancellationToken ct = default)
        {
            return await _db.Devices.AsNoTracking().ToListAsync(ct);
        }

        public async Task UpdateAsync(DeviceModel device, CancellationToken ct = default)
        {
            _db.Devices.Update(device);
            await _db.SaveChangesAsync(ct);
        }
    }
}
