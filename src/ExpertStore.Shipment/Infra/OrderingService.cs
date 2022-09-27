using ExpertStore.Shipment.Application.Integration;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Configuration;

namespace ExpertStore.Shipment.Infra;

public class OrderingService : IOrderingService
{
    private readonly string OrderingServiceUrl;

    public OrderingService(IConfiguration config) 
        => OrderingServiceUrl = config.GetValue<string>("OrdersServiceUrl");

    public async Task<Domain.Shipment?> GetShipmentFromOrderDetails(Guid orderId)
    {
        var orderDetail = await OrderingServiceUrl
            .AppendPathSegment($"/orders/{orderId}")
            .GetJsonAsync<OrderDetail>();
        if (orderDetail is null)
            return null;
        return new Domain.Shipment(
            orderDetail.Id, 
            new(orderDetail.Retailer.Name, orderDetail.Retailer.Address), 
            new(orderDetail.Shopper.Name, orderDetail.Shopper.Address)
        );
    }
}

internal record OrderDetail(
    Guid Id,
    string Status,
    DateTime Date,
    Person Retailer,
    Person Shopper,
    Delivery? Delivery,
    IEnumerable<Item> Items); 

internal record Person(int Id, string Name, string Address);

internal record Item(int ProductId, int Quantity, decimal Value);

internal record Delivery(string Status);

internal record GetOrderInput(Guid OrderId);
