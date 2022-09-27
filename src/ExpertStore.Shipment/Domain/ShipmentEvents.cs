namespace ExpertStore.Shipment.Domain;

public class ShipmentEvents
{
    public ShipmentEvents(DateTime date, string code, string description, string location)
    {
        Date = date;
        Code = code;
        Description = description;
        Location = location;
    }

    public DateTime Date { get; }
    public string Code { get; }
    public string Description { get; }
    public string Location { get; }
}