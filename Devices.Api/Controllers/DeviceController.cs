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

        // GET /Device
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeviceModel>>> GetAll(CancellationToken ct)
        {
            _logger.LogInformation("GetAll devices endpoint called.");
            var devices = await _deviceServices.ListAsync(ct);
            return Ok(devices);
        }

        // GET /Device/{id}
        [HttpGet("{id:guid}", Name = "GetDeviceById")]
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

        // PATCH /Device/{id}
        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] DeviceModel device, CancellationToken ct)
        {
            if (device is null) return BadRequest();

            var existing = await _deviceServices.GetByIdAsync(id, ct);
            if (existing is null) return NotFound();

            device.Id = id;
            device.CreationTime = existing.CreationTime; // preserve original creation time

            await _deviceServices.UpdateAsync(device, ct);

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
