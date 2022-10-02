using ExpertStore.Shipment.Application.Integration;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using PactNet;
using Xunit.Abstractions;
using ExpertStore.Shipment.Infra;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;
using System.Data.Common;
using Flurl.Http;
using ExpertStore.Shipment.Configuration;
using PactNet.Matchers;
using Match = PactNet.Matchers.Match;

namespace ExpertStore.Shipment.UnitTests.Infra;
public class CarrierServiceTests
{
    private readonly IPactBuilderV3 PactBuilder;
    public CarrierServiceTests(ITestOutputHelper output)
    {
        var config = new PactConfig
        {
            PactDir = "../../../../../pacts/",
            DefaultJsonSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }
        };
        IPactV3 pact = Pact.V3("ExpertStore-Shipment", "External-TheCarrier", config);
        PactBuilder = pact.WithHttpInteractions();
        FlurlHttp.Configure(settings => settings.JsonSerializer = new JsonNetSerializer());
    }

    [Fact]
    public async Task GetOrderDetails_OrderExists_ReturnOrder()
    {
        var orderDtoExample = new OrderDto(
            new Guid("d0389a71-e1c5-4667-b068-7466c12c433a"),
            "Approved",
            DateTime.Now.AddDays(-1),
            new(1, "Nike", "NY - NY US 10892"),
            new(2, "Wilson", "SP - SP BR 75880-000"),
            null,
            new Item[] {
                new(1, 10, 100),
                new(2, 1, 200)});

        var registerShipmentInputDto = new RegisterShipmentInputDto
        {
            InsuranceValue = orderDtoExample.Items.Aggregate((decimal)0, (value, item) => value + item.Value),
            LineItems = orderDtoExample.Items.Select(x => $"Product #{x.ProductId} - {x.Value}").ToList(),
            FromAddress = orderDtoExample.Retailer.Address,
            ToAddress = orderDtoExample.Shopper.Address
        };

        var expectedOutput = new RegisterShipmentResponseDto()
        {
            Id = "55c90a1f-5",
            Cost = 5,
            FromAddress = registerShipmentInputDto.FromAddress,
            ToAddress = registerShipmentInputDto.ToAddress,
            Status = "Registered",
            InsuranceValue = registerShipmentInputDto.InsuranceValue,
            LineItems = registerShipmentInputDto.LineItems,
            Label = Guid.NewGuid().ToString().Replace("-", ":label:")
        };

        PactBuilder
            .UponReceiving("A POST to register a shipment in the Carrier")
                .Given("This is a valid shipment data")
                .WithRequest(HttpMethod.Post, "/shipments")
                .WithJsonBody(new TypeMatcher(registerShipmentInputDto), "application/json")
            .WillRespond()
                .WithStatus(System.Net.HttpStatusCode.Created)
                .WithHeader("Content-Type", "application/json; charset=utf-8")
                .WithJsonBody(new TypeMatcher(new { 
                    Id = Match.Type(expectedOutput.Id),
                    Cost = Match.Type(expectedOutput.Cost),
                    ToAddress = Match.Type(registerShipmentInputDto.ToAddress),
                    Status = Match.Regex("Registered", "Registered|Undefined"),
                    InsuranceValue = Match.Type(registerShipmentInputDto.InsuranceValue),
                    LineItems = Match.Type(registerShipmentInputDto.LineItems),
                    Label = Match.Type(expectedOutput.Label)
                }));

        await PactBuilder.VerifyAsync(async ctx =>
        {
            var configKeyValues = new Dictionary<string, string> { { "ExternalCarrierUrl", ctx.MockServerUri.ToString() } };
            var config = (new ConfigurationBuilder()).AddInMemoryCollection(configKeyValues).Build();

            var service = new CarrierService(config, Mock.Of<ILogger<CarrierService>>());
            var (shipmentId, label) = await service.RegisterShipment(orderDtoExample);

            shipmentId.Should().Be(expectedOutput.Id);
            label.Should().Be(expectedOutput.Label);
        });
    }
}
