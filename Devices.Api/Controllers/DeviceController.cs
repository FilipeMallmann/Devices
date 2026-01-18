using Devices.Application.Dtos.Device;
using Devices.Application.Interfaces;
using Devices.Domain;
using Devices.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Devices.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly ILogger<DeviceController> _logger;
        private readonly IDeviceServices _deviceServices;

        public DeviceController(ILogger<DeviceController> logger, IDeviceServices deviceServices)
        {
            _logger = logger;
            _deviceServices = deviceServices ?? throw new ArgumentNullException(nameof(deviceServices));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeviceModel>>> GetAll(CancellationToken ct)
        {
            _logger.LogInformation("GetAll devices endpoint called.");
            var devices = await _deviceServices.ListAsync(ct);
            return Ok(devices);
        }

        // GET /Device/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<DeviceModel>> GetById([FromRoute] Guid id, CancellationToken ct)
        {
            _logger.LogInformation("GetDeviceById endpoint called for id {Id}.", id);
            var device = await _deviceServices.GetByIdAsync(id, ct);
            if (device is null) return NotFound();
            return Ok(device);
        }

        /// <summary>
        /// Create a new device
        /// </summary>
        /// <param name="device"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<DeviceModel>> Create([FromBody] DeviceModel device, CancellationToken ct)
        {
            if (device is null) return BadRequest();

            device.Id = Guid.NewGuid();
            device.CreationTime = DateTime.UtcNow;

            await _deviceServices.AddAsync(device, ct);

            _logger.LogInformation("Created device with id {Id}.", device.Id);
            return CreatedAtRoute("GetDeviceById", new { id = device.Id }, device);
        }

        /// <summary>
        /// Updates the specified device with the provided changes.
        /// </summary>
        /// <param name="id">The unique identifier of the device to update.</param>
        /// <param name="device">An object containing the fields to update for the device. Fields that are null or not set will be left
        /// unchanged.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the update operation. Returns <see
        /// cref="NoContentResult"/> if the update is successful, <see cref="NotFoundResult"/> if the device does not
        /// exist, or <see cref="BadRequestResult"/> if the input is invalid.</returns>
        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateDevicePatchDto device, CancellationToken ct)
        {
            if (device is null) return BadRequest();

            var existing = await _deviceServices.GetByIdAsync(id, ct);
            if (existing is null) return NotFound();

            if (device.State.HasValue)
            {
                existing.State = device.State.Value;
            }
            if (device.Name is not null)
            {
                existing.Name = device.Name;
            }
            if (device.Brand is not null)
            {
                existing.Brand = device.Brand;
            }

            await _deviceServices.UpdateAsync(existing, ct);

            _logger.LogInformation("Updated device with id {Id}.", id);
            return NoContent();
        }

        // DELETE /Device/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
        {
            var existing = await _deviceServices.GetByIdAsync(id, ct);
            if (existing is null) return NotFound();

            await _deviceServices.DeleteAsync(id, ct);

            _logger.LogInformation("Deleted device with id {Id}.", id);
            return NoContent();
        }
    }
}
