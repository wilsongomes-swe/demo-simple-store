namespace ExpertStore.Shipment.Domain;

public interface IShipmentRepository
{
    Task<List<Shipment>> GetList();
    Task<Shipment?> Get(Guid id);
    Task Update(Shipment shipment);
    Task Save(Shipment shipment);
}