using ExpertStore.Ordering.Domain;
using ExpertStore.SeedWork.Interfaces;

namespace ExpertStore.Ordering.UseCases;

public class ListOrders : IUseCase<List<ListOrdersOutputItem>>
{
    public ListOrders(IOrderRepository repository, ILogger<ListOrders> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<ListOrdersOutputItem>> Handle()
    {
        var orders = await _repository.GetList();
        _logger.LogInformation("Orders listed");
        return orders.Select(ListOrdersOutputItem.FromOrder).ToList();
    }

    private readonly IOrderRepository _repository;
    private readonly ILogger<ListOrders> _logger;
}

public record Person(int Id, string Name, string Address);
public record Item(int ProductId, int Quantity, decimal Value);
public record Delivery(string CarrierReference, string Status);

public record ListOrdersOutputItem(
    Guid Id,
    string Status,
    DateTime Date,
    Person Retailer,
    Person Shopper,
    Delivery? Delivery,
    IEnumerable<Item> Items) {

    public static ListOrdersOutputItem FromOrder(Order order)
        => new(order.Id, order.Status.ToString(), order.Date,
            new(order.Retailer.Id, order.Retailer.Name, order.Retailer.Address),
            new(order.Shopper.Id, order.Shopper.Name, order.Shopper.Address),
            order.Delivery is null ? null : new(order.Delivery.CarrierReference, order.Delivery.CurrentStatus.ToString()),
            order.Items.Select(item => new Item(item.ProductId, item.Quantity, item.Value)));
};
