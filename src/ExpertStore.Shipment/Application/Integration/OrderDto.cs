namespace ExpertStore.Shipment.Application.Integration;

public record OrderDto(
    Guid Id,
    string Status,
    DateTime Date,
    Person Retailer,
    Person Shopper,
    Delivery? Delivery,
    IEnumerable<Item> Items);

public record Person(int Id, string Name, string Address);

public record Item(int ProductId, int Quantity, decimal Value);

public record Delivery(string Status);

public record GetOrderInput(Guid OrderId);