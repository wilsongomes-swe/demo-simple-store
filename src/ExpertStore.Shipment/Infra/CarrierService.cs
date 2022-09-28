using ExpertStore.Shipment.Application.Integration;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;

namespace ExpertStore.Shipment.Infra;

public class CarrierService : ICarrierService
{
    private readonly string CarrierUrl;
    private readonly ILogger<CarrierService> _logger;

    public CarrierService(IConfiguration config, ILogger<CarrierService> logger)
    {
        CarrierUrl = config.GetValue<string>("ExternalCarrierUrl");
        _logger = logger;
    }

    public async Task<(string, string)> RegisterShipment(OrderDto orderDto)
    {
        _logger.LogInformation($"Registering shipment from order: {orderDto.Id}");
        var registeredShipmentJson = await CarrierUrl
            .AppendPathSegment("shipments")
            .PostJsonAsync(new RegisterShipmentInputDto { 
                InsuranceValue = orderDto.Items.Aggregate((decimal)0, (value, item) => value + item.Value),
                LineItems = orderDto.Items.Select(x => $"Product #{x.ProductId} - {x.Value}").ToList(),
                FromAddress = orderDto.Retailer.Address,
                ToAddress = orderDto.Shopper.Address
            }).ReceiveString();
        var registeredShipment = JsonConvert.DeserializeObject<RegisterShipmentResponseDto>(registeredShipmentJson);
        _logger.LogInformation($"Registered! Shipment id: {registeredShipment.Id}");
        return (registeredShipment.Id, registeredShipment.Label);
    }
}

internal class RegisterShipmentInputDto
{
    public string FromAddress { get; set; }
    public string ToAddress { get; set; }
    public decimal InsuranceValue { get; set; }
    public List<string> LineItems { get; set; }
}

internal class RegisterShipmentResponseDto
{
    public string Id { get; set; }
    public string FromAddress { get; set; }
    public string ToAddress { get; set; }
    public string Status { get; set; }
    public string Label { get; set; }
    public decimal InsuranceValue { get; set; }
    public List<string> LineItems { get; set; }
    public decimal Cost { get; set; }
}