using Microsoft.AspNetCore.Mvc;

namespace External.TheCarrier;

[ApiController, Route("[controller]")]
public class ShipmentsController : ControllerBase
{
    public ShipmentsController(ILogger<ShipmentsController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public IActionResult Register(CreateShipmentInput input)
    {
        var shipment = new Shipment
        {
            Id = Guid.NewGuid().ToString().Substring(0, 10),
            Cost = 5,
            FromAddress = input.FromAddress,
            ToAddress = input.ToAddress,
            Status = "Registered",
            InsuranceValue = input.InsuranceValue,
            LineItems = input.LineItems,
            Label = Guid.NewGuid().ToString().Replace("-", ":label:")
        };

        _shipments.Add(shipment);
        return Created("", shipment);
    }

    [HttpGet]
    public IActionResult List() => Ok(_shipments);

    static readonly List<Shipment> _shipments = new();

    readonly ILogger<ShipmentsController> _logger;
}

public class CreateShipmentInput
{
    public string FromAddress { get; set; }
    public string ToAddress { get; set; }
    public decimal InsuranceValue { get; set; }
    public List<string> LineItems { get; set; }
}

public class Shipment
{
    public string Id { get; set; }
    public string FromAddress { get; set; }
    public string ToAddress { get; set; }
    public string Status { get; set; }
    public string Label { get; set; }
    public decimal InsuranceValue { get; set; }
    public List<string> LineItems { get; set; }
    public decimal Cost { get; set; }
}
