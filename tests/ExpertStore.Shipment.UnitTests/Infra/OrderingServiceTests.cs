using PactNet;

namespace ExpertStore.Shipment.UnitTests.Infra;

public class OrderingServiceTests
{
    private readonly IPactBuilderV3 pactBuilder;

    public OrderingServiceTests(IPactBuilderV3 pactBuilder)
    {
        var pact = Pact.V3("shipment API Consumer", "Orders API", new PactConfig());
        this.pactBuilder = pact.WithHttpInteractions();
    }

    [Fact]
    public async Task GetOrderDetails_OrderExists_ReturnOrder()
    {

    }
}
