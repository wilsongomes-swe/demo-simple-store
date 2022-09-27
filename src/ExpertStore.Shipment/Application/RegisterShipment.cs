using ExpertStore.SeedWork.Interfaces;
using ExpertStore.Shipment.Application.Integration;
using ExpertStore.Shipment.Domain;

namespace ExpertStore.Shipment.Application;

public class RegisterShipment : IUseCase<RegisterShipmentInput, RegisterShipmentOutput>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly ILogger<GetShipmentDetail> _logger;
    private readonly IOrderingService _orderingService;
    private readonly ICarrierSerice _carrierService;

    public RegisterShipment(
        IShipmentRepository shipmentRepository,
        IOrderingService orderingService,
        ICarrierSerice carrierService,
        ILogger<GetShipmentDetail> logger)
    {
        _shipmentRepository = shipmentRepository;
        _orderingService = orderingService;
        _carrierService = carrierService;
        _logger = logger;
    }

    public async Task<RegisterShipmentOutput> Handle(RegisterShipmentInput input)
    {
        var existingShipment = await _shipmentRepository.Get(input.OrderId);
        if (existingShipment is not null)
            throw new Exception($"This shipment is already registered, id: {existingShipment.Id}");

        var shipment = _orderingService.GetShipmentFromOrderDetails(input.OrderId);
        if (shipment is null)
            throw new Exception($"Shipment {input.OrderId} doesn`t exists in the orders service");

        var (carrierReference, label) = _carrierService.RegisterShipment(shipment);
        shipment.UpdateAsSentToCarrier(carrierReference, label);

        await _shipmentRepository.Save(shipment);
        return new RegisterShipmentOutput(shipment.Id, shipment.CarrierReference!, shipment.OrderId);
    }
}

public record RegisterShipmentInput(Guid OrderId);

public record RegisterShipmentOutput(Guid ShipmentId, string CarrierReference, Guid OrderId);
