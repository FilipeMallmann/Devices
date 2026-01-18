using Devices.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Devices.Application.Interfaces
{
    public interface IDeviceServices
    {
        Task AddAsync(DeviceModel device, CancellationToken ct = default);

        Task DeleteAsync(Guid id, CancellationToken ct = default);

        Task<DeviceModel?> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task<IEnumerable<DeviceModel>> ListAsync(CancellationToken ct = default);

        Task UpdateAsync(DeviceModel device, CancellationToken ct = default);
    }
}
