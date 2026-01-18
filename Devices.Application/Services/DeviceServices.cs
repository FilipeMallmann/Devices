using Devices.Application.Interfaces;
using Devices.Domain;
using Devices.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Devices.Application.Services
{
    public class DeviceServices : IDeviceServices
    {
        private readonly IDeviceRepository _repository;

        public DeviceServices(IDeviceRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Task AddAsync(DeviceModel device, CancellationToken ct = default)
            => _repository.AddAsync(device, ct);

        public Task DeleteAsync(Guid id, CancellationToken ct = default)
            => _repository.DeleteAsync(id, ct);

        public Task<DeviceModel?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => _repository.GetByIdAsync(id, ct);

        public Task<IEnumerable<DeviceModel>> ListAsync(CancellationToken ct = default)
            => _repository.ListAsync(ct);

        public Task UpdateAsync(DeviceModel device, CancellationToken ct = default)
            => _repository.UpdateAsync(device, ct);

    }
}
