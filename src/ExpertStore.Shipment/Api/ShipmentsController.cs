using ExpertStore.SeedWork.Interfaces;
using ExpertStore.Shipment.Application;
using Microsoft.AspNetCore.Mvc;

namespace ExpertStore.Shipment.Api;

[ApiController, Route("[controller]")]
public class ShipmentsController : ControllerBase
{
    public ShipmentsController(ILogger<ShipmentsController> logger) => _logger = logger;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyCollection<ListShipmentsItem>))]
    public async Task<IActionResult> GetList([FromServices] IUseCase<IReadOnlyCollection<ListShipmentsItem>> useCase)
        => Ok(await useCase.Handle());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(
        [FromRoute] Guid id, 
        [FromServices] IUseCase<GetShipmentDetailInput, ShipmentDetail?> useCase)
    {
        var item = await useCase.Handle(new GetShipmentDetailInput(id));
        if (item is null)
            return NotFound(new ProblemDetails() { Title = $"Shipment '{id}' not found." });

        return Ok(item);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RegisterShipmentOutput))]
    public async Task<IActionResult> Get([FromBody] RegisterShipmentInput input, [FromServices] IUseCase<RegisterShipmentInput, RegisterShipmentOutput> useCase) => Created("", await useCase.Handle(input));

    readonly ILogger<ShipmentsController> _logger;
}
