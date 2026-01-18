using Devices.Api.Mapper;
using Devices.Application.Common;
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
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            _logger.LogInformation("GetAll devices endpoint called.");
            return  (await _deviceServices.ListAsync(ct)).ToActionResult();
        }

        [HttpGet("GetById/{id:guid}", Name = "GetById")]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
        {
            _logger.LogInformation("GetById endpoint called for id {Id}.", id);
           return (await _deviceServices.GetByIdAsync(id, ct)).ToActionResult(); 
        }
        
        [HttpGet("ByBrand/{brand}", Name = "GetByBrand")]
        public async Task<IActionResult> GetByBrand([FromRoute] string brand, CancellationToken ct)
        {
            _logger.LogInformation("GetByBrand endpoint called for brand {brand}.", brand);
           return  (await _deviceServices.ListByBrandAsync(brand, ct)).ToActionResult();
        }

        [HttpGet("ByState/{state:int}", Name = "GetByState")]
        public async Task<IActionResult> GetByState([FromRoute] int state, CancellationToken ct)
        {
            _logger.LogInformation("GetByState endpoint called for state {state}.", state);
           return (await _deviceServices.ListByStateAsync(state, ct)).ToActionResult();
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
            await _deviceServices.AddAsync(device, ct);
            _logger.LogInformation("Created device with id {Id}.", device.Id);
            return CreatedAtRoute("GetById", new { id = device.Id }, device);
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
            var result = await _deviceServices.UpdateAsync(id, device, ct);
            if (result.IsSuccess) _logger.LogInformation("Updated device with id {Id}.", id);
            return result.ToActionResult();
        }

        // DELETE /Device/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
        {
            var result = await _deviceServices.DeleteAsync(id, ct);
            if (result.IsSuccess) _logger.LogInformation("Deleted device with id {Id}.", id);

            return result.ToActionResult();
        }
    }
}
