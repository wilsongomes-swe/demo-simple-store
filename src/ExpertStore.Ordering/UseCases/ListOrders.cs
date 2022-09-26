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

public record ListOrdersOutputItem(
    Guid Id,
    string Status,
    DateTime Date,
    string Retailer,
    string Shopper,
    int TotalItems,
    decimal TotalPrice)
{

    public static ListOrdersOutputItem FromOrder(Order order)
            => new(order.Id, order.Status.ToString(), order.Date,
                order.Retailer.Name,
                order.Shopper.Name,
                order.Items.Aggregate(0, (total, item) => total + item.Quantity),
                order.Items.Aggregate((decimal)0, (total, item) => total + item.Value));
};
