namespace ExpertStore.Shipment.Application.Integration;

public interface ICarrierSerice
{
    public (string, string) RegisterShipment(Domain.Shipment shipment);
}
