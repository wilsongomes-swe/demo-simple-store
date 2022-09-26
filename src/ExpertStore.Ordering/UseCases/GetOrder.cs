using ExpertStore.Ordering.Domain;
using ExpertStore.SeedWork.Interfaces;

namespace ExpertStore.Ordering.UseCases;

public class GetOrder : IUseCase<GetOrderInput, OrderDetail?>
{
    public GetOrder(IOrderRepository repository, ILogger<ListOrders> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<OrderDetail?> Handle(GetOrderInput input)
    {
        var order = await _repository.Get(input.OrderId);
        _logger.LogInformation($"Orders details {input.OrderId}");
        return order is null ? null : OrderDetail.FromOrder(order);
    }

    private readonly IOrderRepository _repository;
    private readonly ILogger<ListOrders> _logger;
}

public record Person(int Id, string Name, string Address);
public record Item(int ProductId, int Quantity, decimal Value);
public record Delivery(string CarrierReference, string Status);

public record GetOrderInput(Guid OrderId);

public record OrderDetail(
    Guid Id,
    string Status,
    DateTime Date,
    Person Retailer,
    Person Shopper,
    Delivery? Delivery,
    IEnumerable<Item> Items)
{

    public static OrderDetail FromOrder(Order order)
        => new(order.Id, order.Status.ToString(), order.Date,
            new(order.Retailer.Id, order.Retailer.Name, order.Retailer.Address),
            new(order.Shopper.Id, order.Shopper.Name, order.Shopper.Address),
            order.Delivery is null ? null : new(order.Delivery.CarrierReference, order.Delivery.CurrentStatus.ToString()),
            order.Items.Select(item => new Item(item.ProductId, item.Quantity, item.Value)));
};
