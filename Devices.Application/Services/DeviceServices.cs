using Devices.Application.Common;
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

        public async Task<ResultWrapper<DeviceModel>> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var device = await _repository.GetByIdAsync(id, ct);
            if (device is null)
            {
                return ResultWrapper<DeviceModel>.Failure(new Error("NotFound", $"Device with Id {id} was not found.",ErrorType.NotFound));
            }
            return ResultWrapper<DeviceModel>.Success(device);
        }

        public Task<ResultWrapper<IEnumerable<DeviceModel>>> ListAsync(CancellationToken ct = default)
        {
            return _repository.ListAsync(ct)
                .ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        return ResultWrapper<IEnumerable<DeviceModel>>.Failure(new Error("RepositoryError", "An error occurred while retrieving devices from the repository.", ErrorType.Unknown));
                    }
                    return ResultWrapper<IEnumerable<DeviceModel>>.Success(task.Result);
                }, ct);
        }

        public async Task<ResultWrapper<IEnumerable<DeviceModel>>> ListByBrandAsync(string brand, CancellationToken ct = default)
        {
            var devices = await _repository.ListByBrandAsync(brand, ct);
            if (devices is null || !devices.Any())
            {
                return ResultWrapper<IEnumerable<DeviceModel>>.Failure(new Error("NotFound", $"No devices found for brand {brand}.", ErrorType.NotFound));
            }

            return ResultWrapper<IEnumerable<DeviceModel>>.Success(devices);

        }

        public async Task<ResultWrapper<IEnumerable<DeviceModel>>> ListByStateAsync(int state, CancellationToken ct = default)
        {
            var devices = await _repository.ListByStateAsync(state, ct);
            if (devices is null || !devices.Any())
            {
                return ResultWrapper<IEnumerable<DeviceModel>>.Failure(new Error("NotFound", $"No devices found in state {state}.", ErrorType.NotFound));
            }

            return ResultWrapper<IEnumerable<DeviceModel>>.Success(devices);
        }

        public Task UpdateAsync(DeviceModel device, CancellationToken ct = default)
        {
            
            return _repository.UpdateAsync(device, ct);

        }

    }
}
