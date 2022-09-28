namespace ExpertStore.Shipment.Application.Integration;

public interface ICarrierService
{
    public Task<(string, string)> RegisterShipment(OrderDto shipment);
}
