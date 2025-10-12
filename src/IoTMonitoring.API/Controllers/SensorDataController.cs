
[ApiController]
[Route("api/[controller]")]
public class SensorDataController : ControllerBase
{
    private readonly ISensorDataService _sensorDataService;

    public SensorDataController(ISensorDataService sensorDataService)
    {
        _sensorDataService = sensorDataService;
    }

    [HttpGet("device/{deviceId}")]
    public async Task<ActionResult<IEnumerable<SensorDataDto>>> GetSensorDataByDevice(
        string deviceId, 
        [FromQuery] DateTime? startDate, 
        [FromQuery] DateTime? endDate)
    {
        var sensorData = await _sensorDataService.GetSensorDataByDeviceAsync(deviceId, startDate, endDate);
        return Ok(sensorData);
    }

    [HttpPost]
    public async Task<ActionResult<SensorDataDto>> AddSensorData(CreateSensorDataDto createSensorDataDto)
    {
        try
        {
            var sensorData = await _sensorDataService.AddSensorDataAsync(createSensorDataDto);
            return CreatedAtAction(nameof(GetSensorDataByDevice), 
                new { deviceId = sensorData.DeviceId }, sensorData);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("bulk")]
    public async Task<IActionResult> AddBulkSensorData(IEnumerable<CreateSensorDataDto> sensorDataDtos)
    {
        try
        {
            await _sensorDataService.AddBulkSensorDataAsync(sensorDataDtos);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}