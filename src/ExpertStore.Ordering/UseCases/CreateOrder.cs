using ExpertStore.Ordering.Domain;
using ExpertStore.SeedWork.IntegrationEvents;
using ExpertStore.SeedWork.Interfaces;

namespace ExpertStore.Ordering.UseCases;

public class CreateOrder : IUseCase<CreateOrderInput, CreateOrderOutput>
{
    public CreateOrder(IOrderRepository orderRepository, IEventBus eventBus, ILogger<CreateOrder> logger)
    {
        _orderRepository = orderRepository;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<CreateOrderOutput> Handle(CreateOrderInput input)
    {
        ValidateInput(input);

        var order = new Order(
            new Domain.Person(input.RetailerId, input.RetailerName, input.RetailerAddress),
            new Domain.Person(input.ShopperId, input.ShopperName, input.ShopperAddress));
        input.Items.ToList().ForEach(
            item => order.AddItem(item.ProductId, item.Quantity, item.Value));

        await _orderRepository.Save(order);

        _eventBus.Publish(new OrderCreatedEvent(
            order.Id,
            order.Date,
            order.Items.Aggregate((decimal)0, (total, item) => total + item.Value))
        );

        _logger.LogInformation($"Order created: {order.Id}");
        return new CreateOrderOutput(order.Id, order.Status.ToString());
    }

    void ValidateInput(CreateOrderInput input)
    {
        if (input.Items.Any(item => item.Quantity == 0)) throw new ArgumentException("Please provide a valid quantity.", nameof(Item.Quantity));
    }

    readonly IOrderRepository _orderRepository;
    readonly IEventBus _eventBus;
    readonly ILogger<CreateOrder> _logger;
}

public record CreateOrderInputItem(int ProductId, int Quantity, decimal Value);

public record CreateOrderInput(
    string ShopperName,
    string ShopperAddress,
    int ShopperId,
    string RetailerName,
    string RetailerAddress,
    int RetailerId,
    IEnumerable<CreateOrderInputItem> Items);

public record CreateOrderOutput(Guid Id, string Status);