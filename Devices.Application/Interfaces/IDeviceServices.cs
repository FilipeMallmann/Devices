using Devices.Application.Common;
using Devices.Application.Dtos.Device;
using Devices.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Devices.Application.Interfaces
{
    public interface IDeviceServices
    {
        Task<ResultWrapper> AddAsync(DeviceModel device, CancellationToken ct = default);
        Task<ResultWrapper> DeleteAsync(Guid id, CancellationToken ct = default);
        Task<ResultWrapper> UpdateAsync(Guid id, UpdateDevicePatchDto device, CancellationToken ct = default);

        Task<ResultWrapper<DeviceModel>> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<ResultWrapper<IEnumerable<DeviceModel>>> ListAsync(CancellationToken ct = default);
        Task<ResultWrapper<IEnumerable<DeviceModel>>> ListByStateAsync(int state, CancellationToken ct = default);
        Task<ResultWrapper<IEnumerable<DeviceModel>>> ListByBrandAsync(string brand, CancellationToken ct = default);
    }
}
