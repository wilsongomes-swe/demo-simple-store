using ExpertStore.Shipment.Application.Integration;
using Flurl;
using Flurl.Http;

namespace ExpertStore.Shipment.Infra;

public class OrderingService : IOrderingService
{
    private readonly string OrderingServiceUrl;
    private readonly ILogger<OrderingService> _logger; 

    public OrderingService(IConfiguration config, ILogger<OrderingService> logger) 
        => (OrderingServiceUrl, _logger) = (config.GetValue<string>("OrdersServiceUrl"), logger);

    public async Task<OrderDto?> GetShipmentFromOrderDetails(Guid orderId)
    {
        _logger.LogInformation($"Calling ordering service to get the order: {orderId}");
        var orderDto = await OrderingServiceUrl
            .AppendPathSegment($"/orders/{orderId}")
            .GetJsonAsync<OrderDto>();
        _logger.LogInformation($"Get response ok with orderId {orderDto.Id}");
        return orderDto;
    }
}

