using ExpertStore.Shipment.Application.Integration;
using ExpertStore.Shipment.Infra;
using FluentAssertions;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PactNet;
using Xunit.Abstractions;
using PactNet.Infrastructure.Outputters;

namespace ExpertStore.Shipment.UnitTests.Infra;

public class OrderingServiceTests
{
    private readonly IPactBuilderV3 PactBuilder;

    public OrderingServiceTests(ITestOutputHelper output)
    {
        var config = new PactConfig {
            PactDir = "../../../../../pacts/",
            DefaultJsonSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }};
        IPactV3 pact = Pact.V3("ExpertStore-Shipment", "ExpertStore-Shipments", config);
        PactBuilder = pact.WithHttpInteractions();
    }

    [Fact]
    public async Task GetOrderDetails_OrderExists_ReturnOrder()
    {
        var exampleOrderId = new Guid("d0389a71-e1c5-4667-b068-7466c12c433a");
        var expectedResponse = new OrderDto(
            exampleOrderId,
            "Approved",
            DateTime.Now.AddDays(-1),
            new(1, "Nike", "NY - NY US 10892"),
            new(2, "Wilson", "SP - SP BR 75880-000"),
            null,
            new Item[] {
                new(1, 10, 100),
                new(2, 1, 200)});

        PactBuilder
            .UponReceiving("A GET to get the order details")
                .Given("There is an order with the specified ID")
                .WithRequest(HttpMethod.Get, $"/api/orders/{exampleOrderId}")
            .WillRespond()
                .WithStatus(System.Net.HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json; charset=utf-8")
                .WithJsonBody(expectedResponse);

        await PactBuilder.VerifyAsync(async ctx =>
        {
            var configKeyValues = new Dictionary<string, string> {{ "OrdersServiceUrl", ctx.MockServerUri.ToString() }};
            var config = (new ConfigurationBuilder()).AddInMemoryCollection(configKeyValues).Build();

            var service = new OrderingService(config, Mock.Of<ILogger<OrderingService>>());
            var response = await service.GetOrderDetails(exampleOrderId);

            response.Should().BeEquivalentTo(expectedResponse);
        });
    }
}
