namespace ExpertStore.Shipment.Domain;

public class Address
{
    public Address(string name, string addressLine)
    {
        Name = name;
        AddressLine = addressLine;
    }

    public string Name { get; }
    public string AddressLine { get; }
}