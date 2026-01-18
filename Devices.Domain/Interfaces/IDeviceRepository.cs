using System;
using System.Collections.Generic;
using System.Text;

namespace Devices.Domain.Interfaces
{
    public interface IDeviceRepository
    {
        Task AddAsync(DeviceModel device, CancellationToken ct = default);

        Task DeleteAsync(Guid id, CancellationToken ct = default);

        Task<DeviceModel?> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task<IEnumerable<DeviceModel>> ListAsync(CancellationToken ct = default);

        Task UpdateAsync(DeviceModel device, CancellationToken ct = default);
        Task<IEnumerable<DeviceModel>> ListByBrandAsync(string brand, CancellationToken ct = default);
        Task<IEnumerable<DeviceModel>> ListByStateAsync(int state, CancellationToken ct = default);

    }
}
