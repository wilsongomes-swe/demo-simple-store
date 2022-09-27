namespace ExpertStore.Shipment.Domain;

public enum ShipmentStatus
{
    Pending = 1,
    RegisteredOnCarrier,
    Registered,
    InTransit,
    Delivered,
    Error
}