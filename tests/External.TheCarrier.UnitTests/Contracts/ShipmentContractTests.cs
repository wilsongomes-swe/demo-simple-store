using Microsoft.AspNetCore.Hosting;
using PactNet;
using PactNet.Infrastructure.Outputters;
using PactNet.Verifier;
using ProviderContractTests.Shared;
using Xunit.Abstractions;

namespace ExpertStore.Ordering.UnitTests.Contracts;

public class ShipmentContractTests : IClassFixture<ApiFixture>
{
    private readonly ITestOutputHelper _output;
    private readonly ApiFixture _api;

    public ShipmentContractTests(ApiFixture api, ITestOutputHelper output)
    {
        _output = output;
        _api = api;
        api.CreateClient();
    }

    [Fact]
    public void VerifyLatestPacts()
    {
        var config = new PactVerifierConfig
        {
            LogLevel = PactLogLevel.Information,
            Outputters = new List<IOutput>
            {
                new XUnitOutput(_output),
            }
        };

        IPactVerifier verifier = new PactVerifier(config);

        verifier
            .ServiceProvider("External-TheCarrier", _api.Uri)
            .WithFileSource(new FileInfo(Path.Combine("..", "..", "..", "..", "..", "pacts", "ExpertStore-Shipment-External-TheCarrier.json")))
            .Verify();
    }
}

public class ApiFixture : WebApplicationFactoryWithRealServer<Program> {}