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
        /// <summary>
        /// Retrieves all devices.
        /// </summary>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>An <see cref="IActionResult"/> containing the list of all devices.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            _logger.LogInformation("GetAll devices endpoint called.");
            return (await _deviceServices.ListAsync(ct)).ToActionResult();
        }
        /// <summary>
        /// Retrieves a device by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the device to retrieve.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>An <see cref="IActionResult"/> containing the device data if found; otherwise, a result indicating that the
        /// device was not found.</returns>
        [HttpGet("GetById/{id:guid}", Name = "GetById")]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
        {
            _logger.LogInformation("GetById endpoint called for id {Id}.", id);
            return (await _deviceServices.GetByIdAsync(id, ct)).ToActionResult();
        }
        /// <summary>
        /// Retrieves a list of devices that match the specified brand.
        /// </summary>
        /// <param name="brand">The brand name used to filter the devices. Cannot be null or empty.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>An <see cref="IActionResult"/> containing the list of devices for the specified brand. Returns a 200 OK
        /// response with the devices if found; otherwise, returns a 404 Not Found response if no devices match the
        /// brand.</returns>
        [HttpGet("ByBrand/{brand}", Name = "GetByBrand")]
        public async Task<IActionResult> GetByBrand([FromRoute] string brand, CancellationToken ct)
        {
            _logger.LogInformation("GetByBrand endpoint called for brand {brand}.", brand);
            return (await _deviceServices.ListByBrandAsync(brand, ct)).ToActionResult();
        }
        /// <summary>
        /// Retrieves a list of devices that match the specified state.
        /// </summary>
        /// <param name="state">The state value used to filter devices. Only devices with this state will be included in the results.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>An <see cref="IActionResult"/> containing the list of devices in the specified state. Returns an empty list
        /// if no devices are found.</returns>
        [HttpGet("ByState/{state:int}", Name = "GetByState")]
        public async Task<IActionResult> GetByState([FromRoute] int state, CancellationToken ct)
        {
            _logger.LogInformation("GetByState endpoint called for state {state}.", state);
            return (await _deviceServices.ListByStateAsync(state, ct)).ToActionResult();
        }



        /// <summary>
        /// Creates a new device and returns the created device with its assigned identifier.
        /// </summary>
        /// <param name="device">The device to create. The device information is provided in the request body. Cannot be null.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>An ActionResult containing the created device and a location header with the URI of the new resource. Returns
        /// BadRequest if the device is null.</returns>
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

        /// <summary>
        /// Deletes the device with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the device to delete.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>An <see cref="IActionResult"/> that indicates the result of the delete operation. Returns 200 OK if the device was
        /// deleted successfully; otherwise, returns an appropriate error response.</returns>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
        {
            var result = await _deviceServices.DeleteAsync(id, ct);
            if (result.IsSuccess) _logger.LogInformation("Deleted device with id {Id}.", id);

            return result.ToActionResult();
        }
    }
}
