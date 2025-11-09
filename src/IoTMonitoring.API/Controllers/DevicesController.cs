[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly IDeviceService _deviceService;

    public DevicesController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeviceDto>>> GetDevices()
    {
        var devices = await _deviceService.GetAllDevicesAsync();
        return Ok(devices);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DeviceDto>> GetDevice(int id)
    {
        var device = await _deviceService.GetDeviceByIdAsync(id);
        if (device == null) return NotFound();
        return Ok(device);
    }

    [HttpGet("by-deviceid/{deviceId}")]
    public async Task<ActionResult<DeviceDto>> GetDeviceByDeviceId(string deviceId)
    {
        var device = await _deviceService.GetDeviceByDeviceIdAsync(deviceId);
        if (device == null) return NotFound();
        return Ok(device);
    }

    [HttpPost]
    public async Task<ActionResult<DeviceDto>> CreateDevice(CreateDeviceDto createDeviceDto)
    {
        var device = await _deviceService.CreateDeviceAsync(createDeviceDto);
        return CreatedAtAction(nameof(GetDevice), new { id = device.Id }, device);
    }

    [HttpPut("{deviceId}/status")]
    public async Task<IActionResult> UpdateDeviceStatus(string deviceId, [FromBody] string status)
    {
        if (!Enum.TryParse<DeviceStatus>(status, out var deviceStatus))
        {
            return BadRequest("Invalid status value");
        }

        await _deviceService.UpdateDeviceStatusAsync(deviceId, deviceStatus);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDevice(int id)
    {
        var result = await _deviceService.DeleteDeviceAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}