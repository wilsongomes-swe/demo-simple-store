namespace ExpertStore.Shipment.Application.Integration;

public interface IOrderingService
{
    public Task<OrderDto?> GetShipmentFromOrderDetails(Guid orderId);
}
