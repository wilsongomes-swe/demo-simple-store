namespace ExpertStore.Shipment.Application.Integration;

public interface IOrderingService
{
    public Task<Domain.Shipment?> GetShipmentFromOrderDetails(Guid orderId);
}
