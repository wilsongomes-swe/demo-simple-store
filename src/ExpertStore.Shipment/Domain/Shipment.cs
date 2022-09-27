namespace ExpertStore.Shipment.Domain;

public class Shipment
{
    public Shipment(
        Guid orderId,
        Address consignee,
        Address consignor)
    {
        Id = Guid.NewGuid();
        RegisteredAt = DateTime.Now;
        OrderId = orderId;
        Consignee = consignee;
        Consignor = consignor;
        Status = ShipmentStatus.Pending;
        _events = new List<ShipmentEvents>();
        _events.Add(new ShipmentEvents(DateTime.Now, "SC", "Shipment Created", ""));
    }

    public void UpdateAsSentToCarrier(string carrierReference, string label)
    {
        Status = ShipmentStatus.Registered;
        CarrierReference = carrierReference;
        Label = label;
    }

    public Guid Id { get; }
    public Guid OrderId { get; }
    public DateTime RegisteredAt { get; }
    public string? label { get; }
    public string? CarrierReference { get; private set; }
    public string? Label { get; private set; }
    public Address Consignee { get; }
    public Address Consignor { get; }
    public ShipmentStatus Status { get; private set; }
    public IReadOnlyCollection<ShipmentEvents> Events => _events.AsReadOnly();

    readonly List<ShipmentEvents> _events;
}