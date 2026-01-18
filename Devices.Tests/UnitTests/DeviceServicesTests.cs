using System;
using System.Collections.Generic;
using System.Text;
using Devices.Application.Common;
using Devices.Application.Dtos.Device;
using Devices.Application.Interfaces;
using Devices.Application.Services;
using Devices.Domain;
using Devices.Domain.Interfaces;
using FluentAssertions;
using NSubstitute;
using static Devices.Domain.Enums;

namespace Devices.Tests.UnitTests
{
    public class DeviceServicesTests
    {
        private readonly IDeviceRepository _mockRepository;
        private readonly DeviceServices _sut;

        public DeviceServicesTests()
        {
            _mockRepository = Substitute.For<IDeviceRepository>();
            _sut = new DeviceServices(_mockRepository);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithNullRepository_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new DeviceServices(null!));
        }

        #endregion

        #region AddAsync Tests

        [Fact]
        public async Task AddAsync_WithValidDevice_ShouldAddAndSetIdAndCreationTime()
        {
            // Arrange
            var device = new DeviceModel
            {
                Name = "TestDevice",
                Brand = "TestBrand",
                State = DeviceState.Available
            };

            // Act
            await _sut.AddAsync(device);

            // Assert
            device.Id.Should().NotBe(Guid.Empty);
            device.CreationTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            await _mockRepository.Received(1).AddAsync(device);
        }

        [Fact]
        public async Task AddAsync_WithNullDevice_ReturnsFailureWithValidationError()
        {

            // Act
            var result = await _sut.AddAsync(null!);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error!.Code.Should().Be("InvalidInput");
            result.Error!.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public async Task AddAsync_WithCancellationToken_ShouldPassToRepository()
        {
            // Arrange
            var device = new DeviceModel
            {
                Name = "TestDevice",
                Brand = "TestBrand",
                State = DeviceState.Available
            };
            var cts = new CancellationTokenSource();

            // Act
            await _sut.AddAsync(device, cts.Token);

            // Assert
            await _mockRepository.Received(1).AddAsync(device, cts.Token);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WithExistingDevice_ReturnsSuccessWithDevice()
        {
            // Arrange
            var deviceId = Guid.NewGuid();
            var device = new DeviceModel
            {
                Id = deviceId,
                Name = "TestDevice",
                Brand = "TestBrand",
                State = DeviceState.Available,
                CreationTime = DateTime.UtcNow
            };
            _mockRepository.GetByIdAsync(deviceId).Returns(device);

            // Act
            var result = await _sut.GetByIdAsync(deviceId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(device);
            result.Error.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistentDevice_ReturnsFailureWithNotFoundError()
        {
            // Arrange
            var deviceId = Guid.NewGuid();
            _mockRepository.GetByIdAsync(deviceId).Returns((DeviceModel?)null);

            // Act
            var result = await _sut.GetByIdAsync(deviceId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error!.Code.Should().Be("NotFound");
            result.Error!.Type.Should().Be(ErrorType.NotFound);
            result.Error!.Message.Should().Contain(deviceId.ToString());
        }

        [Fact]
        public async Task GetByIdAsync_WithCancellationToken_ShouldPassToRepository()
        {
            // Arrange
            var deviceId = Guid.NewGuid();
            var cts = new CancellationTokenSource();
            _mockRepository.GetByIdAsync(deviceId, cts.Token).Returns((DeviceModel?)null);

            // Act
            await _sut.GetByIdAsync(deviceId, cts.Token);

            // Assert
            await _mockRepository.Received(1).GetByIdAsync(deviceId, cts.Token);
        }

        #endregion

        #region ListAsync Tests

        [Fact]
        public async Task ListAsync_WithDevices_ReturnsSuccessWithDevices()
        {
            // Arrange
            var devices = new List<DeviceModel>
            {
                new() { Id = Guid.NewGuid(), Name = "Device1", Brand = "Brand1", State = DeviceState.Available },
                new() { Id = Guid.NewGuid(), Name = "Device2", Brand = "Brand2", State = DeviceState.InUse }
            };
            _mockRepository.ListAsync().Returns(devices);

            // Act
            var result = await _sut.ListAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(devices);
            result.Error.Should().BeNull();
        }

        [Fact]
        public async Task ListAsync_WithEmptyList_ReturnsSuccessWithEmptyList()
        {
            // Arrange
            var devices = new List<DeviceModel>();
            _mockRepository.ListAsync().Returns(devices);

            // Act
            var result = await _sut.ListAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEmpty();
        }

        [Fact]
        public async Task ListAsync_WhenRepositoryThrowsException_ReturnsFailureWithRepositoryError()
        {
            // Arrange
            _mockRepository.ListAsync().Returns(Task.FromException<IEnumerable<DeviceModel>>(new Exception("Database error")));

            // Act
            var result = await _sut.ListAsync();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error!.Code.Should().Be("RepositoryError");
            result.Error!.Type.Should().Be(ErrorType.Unknown);
        }

        [Fact]
        public async Task ListAsync_WithCancellationToken_ShouldPassToRepository()
        {
            // Arrange
            var devices = new List<DeviceModel>();
            var cts = new CancellationTokenSource();
            _mockRepository.ListAsync(cts.Token).Returns(devices);

            // Act
            await _sut.ListAsync(cts.Token);

            // Assert
            await _mockRepository.Received(1).ListAsync(cts.Token);
        }

        #endregion

        #region ListByBrandAsync Tests

        [Fact]
        public async Task ListByBrandAsync_WithExistingBrand_ReturnsSuccessWithDevices()
        {
            // Arrange
            var brand = "TestBrand";
            var devices = new List<DeviceModel>
            {
                new() { Id = Guid.NewGuid(), Name = "Device1", Brand = brand, State = DeviceState.Available },
                new() { Id = Guid.NewGuid(), Name = "Device2", Brand = brand, State = DeviceState.InUse }
            };
            _mockRepository.ListByBrandAsync(brand).Returns(devices);

            // Act
            var result = await _sut.ListByBrandAsync(brand);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(devices);
            result.Error.Should().BeNull();
        }

        [Fact]
        public async Task ListByBrandAsync_WithNonExistentBrand_ReturnsFailureWithNotFoundError()
        {
            // Arrange
            var brand = "NonExistentBrand";
            _mockRepository.ListByBrandAsync(brand).Returns(new List<DeviceModel>());

            // Act
            var result = await _sut.ListByBrandAsync(brand);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error!.Code.Should().Be("NotFound");
            result.Error!.Type.Should().Be(ErrorType.NotFound);
            result.Error!.Message.Should().Contain(brand);
        }

        [Fact]
        public async Task ListByBrandAsync_WithNullResult_ReturnsFailureWithNotFoundError()
        {
            // Arrange
            var brand = "TestBrand";
            _mockRepository.ListByBrandAsync(brand).Returns((IEnumerable<DeviceModel>?)null);

            // Act
            var result = await _sut.ListByBrandAsync(brand);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Code.Should().Be("NotFound");
        }

        [Fact]
        public async Task ListByBrandAsync_WithCancellationToken_ShouldPassToRepository()
        {
            // Arrange
            var brand = "TestBrand";
            var devices = new List<DeviceModel> { new() { Brand = brand } };
            var cts = new CancellationTokenSource();
            _mockRepository.ListByBrandAsync(brand, cts.Token).Returns(devices);

            // Act
            await _sut.ListByBrandAsync(brand, cts.Token);

            // Assert
            await _mockRepository.Received(1).ListByBrandAsync(brand, cts.Token);
        }

        #endregion

        #region ListByStateAsync Tests

        [Fact]
        public async Task ListByStateAsync_WithExistingState_ReturnsSuccessWithDevices()
        {
            // Arrange
            var state = (int)DeviceState.Available;
            var devices = new List<DeviceModel>
            {
                new() { Id = Guid.NewGuid(), Name = "Device1", Brand = "Brand1", State = DeviceState.Available },
                new() { Id = Guid.NewGuid(), Name = "Device2", Brand = "Brand2", State = DeviceState.Available }
            };
            _mockRepository.ListByStateAsync(state).Returns(devices);

            // Act
            var result = await _sut.ListByStateAsync(state);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(devices);
            result.Error.Should().BeNull();
        }

        [Fact]
        public async Task ListByStateAsync_WithNonExistentState_ReturnsFailureWithNotFoundError()
        {
            // Arrange
            var state = (int)DeviceState.Inactive;
            _mockRepository.ListByStateAsync(state).Returns(new List<DeviceModel>());

            // Act
            var result = await _sut.ListByStateAsync(state);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error!.Code.Should().Be("NotFound");
            result.Error!.Type.Should().Be(ErrorType.NotFound);
            result.Error!.Message.Should().Contain(state.ToString());
        }

        [Fact]
        public async Task ListByStateAsync_WithCancellationToken_ShouldPassToRepository()
        {
            // Arrange
            var state = (int)DeviceState.Available;
            var devices = new List<DeviceModel> { new() { State = DeviceState.Available } };
            var cts = new CancellationTokenSource();
            _mockRepository.ListByStateAsync(state, cts.Token).Returns(devices);

            // Act
            await _sut.ListByStateAsync(state, cts.Token);

            // Assert
            await _mockRepository.Received(1).ListByStateAsync(state, cts.Token);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_WithExistingAvailableDevice_ReturnsSuccess()
        {
            // Arrange
            var deviceId = Guid.NewGuid();
            var device = new DeviceModel
            {
                Id = deviceId,
                Name = "TestDevice",
                Brand = "TestBrand",
                State = DeviceState.Available
            };
            _mockRepository.GetByIdAsync(deviceId).Returns(device);

            // Act
            var result = await _sut.DeleteAsync(deviceId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Error.Should().BeNull();
            await _mockRepository.Received(1).DeleteAsync(deviceId);
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistentDevice_ReturnsFailureWithNotFoundError()
        {
            // Arrange
            var deviceId = Guid.NewGuid();
            _mockRepository.GetByIdAsync(deviceId).Returns((DeviceModel?)null);

            // Act
            var result = await _sut.DeleteAsync(deviceId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error!.Code.Should().Be("NotFound");
            result.Error!.Type.Should().Be(ErrorType.NotFound);
            await _mockRepository.DidNotReceive().DeleteAsync(deviceId);
        }

        [Fact]
        public async Task DeleteAsync_WithInUseDevice_ReturnsFailureWithValidationError()
        {
            // Arrange
            var deviceId = Guid.NewGuid();
            var device = new DeviceModel
            {
                Id = deviceId,
                Name = "TestDevice",
                Brand = "TestBrand",
                State = DeviceState.InUse
            };
            _mockRepository.GetByIdAsync(deviceId).Returns(device);

            // Act
            var result = await _sut.DeleteAsync(deviceId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error!.Code.Should().Be("InvalidOperation");
            result.Error!.Type.Should().Be(ErrorType.Validation);
            result.Error!.Message.Should().Contain("active device");
            await _mockRepository.DidNotReceive().DeleteAsync(deviceId);
        }

        [Fact]
        public async Task DeleteAsync_WithInactiveDevice_ReturnsSuccess()
        {
            // Arrange
            var deviceId = Guid.NewGuid();
            var device = new DeviceModel
            {
                Id = deviceId,
                Name = "TestDevice",
                Brand = "TestBrand",
                State = DeviceState.Inactive
            };
            _mockRepository.GetByIdAsync(deviceId).Returns(device);

            // Act
            var result = await _sut.DeleteAsync(deviceId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            await _mockRepository.Received(1).DeleteAsync(deviceId);
        }

        [Fact]
        public async Task DeleteAsync_WithCancellationToken_ShouldPassToRepository()
        {
            // Arrange
            var deviceId = Guid.NewGuid();
            var device = new DeviceModel
            {
                Id = deviceId,
                State = DeviceState.Available
            };
            var cts = new CancellationTokenSource();
            _mockRepository.GetByIdAsync(deviceId, cts.Token).Returns(device);

            // Act
            await _sut.DeleteAsync(deviceId, cts.Token);

            // Assert
            await _mockRepository.Received(1).GetByIdAsync(deviceId, cts.Token);
            await _mockRepository.Received(1).DeleteAsync(deviceId, cts.Token);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WithNullDevice_ReturnsFailureWithValidationError()
        {
            // Arrange
            var deviceId = Guid.NewGuid();

            // Act
            var result = await _sut.UpdateAsync(deviceId, null!);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error!.Code.Should().Be("InvalidInput");
            result.Error!.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistentDevice_ReturnsFailureWithNotFoundError()
        {
            // Arrange
            var deviceId = Guid.NewGuid();
            var updateDto = new UpdateDevicePatchDto { Name = "NewName" };
            _mockRepository.GetByIdAsync(deviceId).Returns((DeviceModel?)null);

            // Act
            var result = await _sut.UpdateAsync(deviceId, updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error!.Code.Should().Be("NotFound");
            result.Error!.Type.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task UpdateAsync_WithAvailableDeviceAndNewName_ReturnsSuccess()
        {
            // Arrange
            var deviceId = Guid.NewGuid();
            var existingDevice = new DeviceModel
            {
                Id = deviceId,
                Name = "OldName",
                Brand = "TestBrand",
                State = DeviceState.Available
            };
            var updateDto = new UpdateDevicePatchDto { Name = "NewName" };
            _mockRepository.GetByIdAsync(deviceId).Returns(existingDevice);

            // Act
            var result = await _sut.UpdateAsync(deviceId, updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            existingDevice.Name.Should().Be("NewName");
            await _mockRepository.Received(1).UpdateAsync(existingDevice);
        }

        [Fact]
        public async Task UpdateAsync_WithAvailableDeviceAndNewBrand_ReturnsSuccess()
        {
            // Arrange
            var deviceId = Guid.NewGuid();
            var existingDevice = new DeviceModel
            {
                Id = deviceId,
                Name = "TestDevice",
                Brand = "OldBrand",
                State = DeviceState.Available
            };
            var updateDto = new UpdateDevicePatchDto { Brand = "NewBrand" };
            _mockRepository.GetByIdAsync(deviceId).Returns(existingDevice);

            // Act
            var result = await _sut.UpdateAsync(deviceId, updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            existingDevice.Brand.Should().Be("NewBrand");
            await _mockRepository.Received(1).UpdateAsync(existingDevice);
        }

        [Fact]
        public async Task UpdateAsync_WithInUseDeviceAndNameChange_ReturnsFailureWithValidationError()
        {
            // Arrange
            var deviceId = Guid.NewGuid();
            var existingDevice = new DeviceModel
            {
                Id = deviceId,
                Name = "TestDevice",
                Brand = "TestBrand",
                State = DeviceState.InUse
            };
            var updateDto = new UpdateDevicePatchDto { Name = "NewName" };
            _mockRepository.GetByIdAsync(deviceId).Returns(existingDevice);

            // Act
            var result = await _sut.UpdateAsync(deviceId, updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error!.Code.Should().Be("InvalidOperation");
            result.Error!.Type.Should().Be(ErrorType.Validation);
            existingDevice.Name.Should().Be("TestDevice"); // Name should not be updated
            await _mockRepository.DidNotReceive().UpdateAsync(Arg.Any<DeviceModel>());
        }

        [Fact]
        public async Task UpdateAsync_WithInUseDeviceAndBrandChange_ReturnsFailureWithValidationError()
        {
            // Arrange
            var deviceId = Guid.NewGuid();
            var existingDevice = new DeviceModel
            {
                Id = deviceId,
                Name = "TestDevice",
                Brand = "TestBrand",
                State = DeviceState.InUse
            };
            var updateDto = new UpdateDevicePatchDto { Brand = "NewBrand" };
            _mockRepository.GetByIdAsync(deviceId).Returns(existingDevice);

            // Act
            var result = await _sut.UpdateAsync(deviceId, updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error!.Code.Should().Be("InvalidOperation");
            result.Error!.Type.Should().Be(ErrorType.Validation);
            existingDevice.Brand.Should().Be("TestBrand"); // Brand should not be updated
            await _mockRepository.DidNotReceive().UpdateAsync(Arg.Any<DeviceModel>());
        }

        [Fact]
        public async Task UpdateAsync_WithInUseDeviceAndOnlyStateChange_ReturnsSuccess()
        {
            // Arrange
            var deviceId = Guid.NewGuid();
            var existingDevice = new DeviceModel
            {
                Id = deviceId,
                Name = "TestDevice",
                Brand = "TestBrand",
                State = DeviceState.InUse
            };
            var updateDto = new UpdateDevicePatchDto { State = DeviceState.Inactive };
            _mockRepository.GetByIdAsync(deviceId).Returns(existingDevice);

            // Act
            var result = await _sut.UpdateAsync(deviceId, updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            existingDevice.State.Should().Be(DeviceState.Inactive);
            await _mockRepository.Received(1).UpdateAsync(existingDevice);
        }

        [Fact]
        public async Task UpdateAsync_WithMultipleChanges_ReturnsSuccess()
        {
            // Arrange
            var deviceId = Guid.NewGuid();
            var existingDevice = new DeviceModel
            {
                Id = deviceId,
                Name = "OldName",
                Brand = "OldBrand",
                State = DeviceState.Available
            };
            var updateDto = new UpdateDevicePatchDto
            {
                Name = "NewName",
                Brand = "NewBrand",
                State = DeviceState.InUse
            };
            _mockRepository.GetByIdAsync(deviceId).Returns(existingDevice);

            // Act
            var result = await _sut.UpdateAsync(deviceId, updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            existingDevice.Name.Should().Be("NewName");
            existingDevice.Brand.Should().Be("NewBrand");
            existingDevice.State.Should().Be(DeviceState.InUse);
            await _mockRepository.Received(1).UpdateAsync(existingDevice);
        }

        [Fact]
        public async Task UpdateAsync_WithEmptyUpdateDto_ReturnsSuccess()
        {
            // Arrange
            var deviceId = Guid.NewGuid();
            var existingDevice = new DeviceModel
            {
                Id = deviceId,
                Name = "TestDevice",
                Brand = "TestBrand",
                State = DeviceState.Available
            };
            var updateDto = new UpdateDevicePatchDto(); // No properties set
            _mockRepository.GetByIdAsync(deviceId).Returns(existingDevice);

            // Act
            var result = await _sut.UpdateAsync(deviceId, updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            existingDevice.Name.Should().Be("TestDevice");
            existingDevice.Brand.Should().Be("TestBrand");
            existingDevice.State.Should().Be(DeviceState.Available);
            await _mockRepository.Received(1).UpdateAsync(existingDevice);
        }

        [Fact]
        public async Task UpdateAsync_WithCancellationToken_ShouldPassToRepository()
        {
            // Arrange
            var deviceId = Guid.NewGuid();
            var existingDevice = new DeviceModel
            {
                Id = deviceId,
                State = DeviceState.Available
            };
            var updateDto = new UpdateDevicePatchDto { State = DeviceState.InUse };
            var cts = new CancellationTokenSource();
            _mockRepository.GetByIdAsync(deviceId, cts.Token).Returns(existingDevice);

            // Act
            await _sut.UpdateAsync(deviceId, updateDto, cts.Token);

            // Assert
            await _mockRepository.Received(1).GetByIdAsync(deviceId, cts.Token);
            await _mockRepository.Received(1).UpdateAsync(existingDevice, cts.Token);
        }

        #endregion
    }
}
