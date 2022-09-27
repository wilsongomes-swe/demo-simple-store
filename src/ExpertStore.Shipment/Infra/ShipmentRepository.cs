using ExpertStore.Shipment.Domain;

namespace ExpertStore.Shipment.Infra;

public class ShipmentRepository : IShipmentRepository
{
    public ShipmentRepository()
    {
        _shipments = new List<Domain.Shipment>();
    }

    public Task<Domain.Shipment?> Get(Guid id)
        => Task.FromResult(_shipments.Find(x => x.Id == id));

    public Task<List<Domain.Shipment>> GetList()
        => Task.FromResult(_shipments.ToList());

    public Task Save(Domain.Shipment shipment)
    {
        _shipments.Add(shipment);
        return Task.CompletedTask;
    }

    public Task Update(Domain.Shipment shipment)
    {
        _shipments.RemoveAll(x => x.Id == shipment.Id);
        _shipments.Add(shipment);
        return Task.CompletedTask;
    }

    readonly List<Domain.Shipment> _shipments;
}