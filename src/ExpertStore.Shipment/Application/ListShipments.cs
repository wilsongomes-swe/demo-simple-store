using ExpertStore.SeedWork.Interfaces;
using ExpertStore.Shipment.Domain;

namespace ExpertStore.Shipment.Application;

public class ListShipments : IUseCase<IReadOnlyCollection<ListShipmentsItem>?>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly ILogger<ListShipments> _logger;

    public ListShipments(IShipmentRepository shipmentRepository, ILogger<ListShipments> logger)
    {
        _shipmentRepository = shipmentRepository;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<ListShipmentsItem>?> Handle()
    {
        var items = await _shipmentRepository.GetList();
        _logger.LogInformation("Listing shipments");
        var output = items?.Select(x => new ListShipmentsItem(
            x.Id,
            x.RegisteredAt,
            x.OrderId.ToString(),
            x.Consignor.AddressLine,
            x.Consignee.AddressLine,
            x.Status.ToString())).ToList().AsReadOnly();
        return output;
    }
}

public record ListShipmentsItem(Guid Id, DateTime RegisteredAt, string OrderId, string From, string To, string Status);
