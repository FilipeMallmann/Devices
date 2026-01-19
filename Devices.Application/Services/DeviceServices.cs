using Devices.Application.Common;
using Devices.Application.Dtos.Device;
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

        public async Task<ResultWrapper> AddAsync(DeviceModel device, CancellationToken ct = default)
        {
            if (device is null)
            {
                return ResultWrapper.Failure(new Error("InvalidInput", "Device model cannot be null.", ErrorType.Validation));
            }
            device.Id = Guid.NewGuid();
            device.CreationTime = DateTime.UtcNow;

            await _repository.AddAsync(device, ct);
            return ResultWrapper.Success();
        }

        public async Task<ResultWrapper> DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var existing = await _repository.GetByIdAsync(id, ct);
            if (existing is null)
            {
                return ResultWrapper.Failure(new Error("NotFound", $"Device with Id {id} was not found.", ErrorType.NotFound));
            }
            if (existing.State == Enums.DeviceState.InUse)
            {
                return ResultWrapper.Failure(new Error("InvalidOperation", $"Cannot delete an active device. Deviceid: {id}", ErrorType.Validation));
            }
            await _repository.DeleteAsync(id, ct);
            return ResultWrapper.Success();


        }

        public async Task<ResultWrapper<DeviceModel>> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var device = await _repository.GetByIdAsync(id, ct);
            if (device is null)
            {
                return ResultWrapper<DeviceModel>.Failure(new Error("NotFound", $"Device with Id {id} was not found.", ErrorType.NotFound));
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

        public async Task<ResultWrapper> UpdateAsync(Guid id, UpdateDevicePatchDto device, CancellationToken ct = default)
        {
            if (device is null)
            {
                return ResultWrapper.Failure(new Error("InvalidInput", "Device model cannot be null.", ErrorType.Validation));
            }

            var existing = await _repository.GetByIdAsync(id, ct);
            if (existing is null)
            {
                return ResultWrapper<IEnumerable<DeviceModel>>.Failure(new Error("NotFound", $"Device with Id {id} was not found.", ErrorType.NotFound));
            }


            if (device.Name is not null && (device.Name != existing.Name))
            {
                if (existing.State == Enums.DeviceState.InUse)
                {
                    return ResultWrapper.Failure(new Error("InvalidOperation", $"Cannot update the name or brand on an active device. Deviceid: {id}", ErrorType.Validation));
                }
                existing.Name = device.Name;
            }
            if (device.Brand is not null && (device.Brand != existing.Brand))
            {
                if (existing.State == Enums.DeviceState.InUse)
                {
                    return ResultWrapper.Failure(new Error("InvalidOperation", $"Cannot update the name or brand on an active device. Deviceid: {id}", ErrorType.Validation));
                }
                existing.Brand = device.Brand;
            }
            if (device.State.HasValue)
            {
                existing.State = device.State.Value;
            }

            await _repository.UpdateAsync(existing, ct);
            return ResultWrapper.Success();
        }

    }
}
