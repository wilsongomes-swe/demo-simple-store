using Microsoft.AspNetCore.Mvc;

namespace ExpertStore.Shipment.Api;

[ApiController, Route("[controller]")]
public class ShipmentsController : ControllerBase
{
    public ShipmentsController(ILogger<ShipmentsController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<string> Get()
    {
        return null;
    }

    readonly ILogger<ShipmentsController> _logger;
}