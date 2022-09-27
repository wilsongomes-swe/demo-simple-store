using ExpertStore.SeedWork.Interfaces;
using ExpertStore.Shipment.Domain;

namespace ExpertStore.Shipment.Application;

public class GetShipmentDetail : IUseCase<GetShipmentDetailInput, ShipmentDetail?>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly ILogger<GetShipmentDetail> _logger;

    public GetShipmentDetail(
        IShipmentRepository shipmentRepository, 
        ILogger<GetShipmentDetail> logger)
    {
        _shipmentRepository = shipmentRepository;
        _logger = logger;
    }

    public async Task<ShipmentDetail?> Handle(GetShipmentDetailInput input)
    {
        _logger.LogInformation($"Shipment details requested {input.ShipmentId}");
        return ShipmentDetail.FromShipment(await _shipmentRepository.Get(input.ShipmentId));
    }
}

public record GetShipmentDetailInput(Guid ShipmentId);

public record ShipmentDetail(
    Guid Id,
    DateTime RegisteredAt,
    Guid OrderId,
    Address Consignee,
    Address Consignor,
    string Status,
    IEnumerable<Event> Events
)
{
    public static ShipmentDetail? FromShipment(Domain.Shipment? shipment)
        => shipment is null ? null : new ShipmentDetail(
            shipment.Id,
            shipment.RegisteredAt,
            shipment.OrderId,
            new(shipment.Consignee.Name, shipment.Consignee.AddressLine),
            new(shipment.Consignor.Name, shipment.Consignor.AddressLine),
            shipment.Status.ToString(),
            shipment.Events.Select(x => new Event(x.Date, x.Code, x.Description, x.Location)));
};

public record Address(string Name, string AddressLine);

public record Event(DateTime Date, string Code, string Description, string Location);

