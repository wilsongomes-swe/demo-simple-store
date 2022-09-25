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
            new(input.RetailerId, input.RetailerName, input.RetailerAddress),
            new(input.ShopperId, input.ShopperName, input.ShopperAddress));
        input.Items.ToList().ForEach(
            item => order.AddItem(item.ProductId, item.Quantity, item.Value));

        await _orderRepository.Save(order);
        
        //_eventBus.Publish(new OrderCreatedEvent(
        //    order.Date,
        //    order.Id,
        //    order.ProductId,
        //    order.Quantity));
        
        _logger.LogInformation("Order created! (log)");
        _logger.LogTrace("Order created! (trace)");
        return new CreateOrderOutput(order.Id, order.Status.ToString());
    }

    void ValidateInput(CreateOrderInput input)
    { }

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
