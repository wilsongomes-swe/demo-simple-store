using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PactNet;
using PactNet.Infrastructure.Outputters;
using PactNet.Verifier;
using Xunit.Abstractions;

namespace ExpertStore.Ordering.UnitTests.Contracts;

public class ShipmentContractTests
{
    private readonly ITestOutputHelper _output;

    public ShipmentContractTests(ITestOutputHelper output)
    {
        _output = output;
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

        verifier.ServiceProvider("ExpertStore-Ordering", new Uri("http://localhost:525"))
                .WithFileSource(new FileInfo(Path.Combine("..", "..", "..", "..", "..", "pacts2", "ExpertStore-Shipment-ExpertStore-Ordering.json")))
                // .WithFilter(providerState: "There is not an order with the specified ID")
                .Verify();
    }
}

public class XUnitOutput : IOutput
{
    private readonly ITestOutputHelper output;

    public XUnitOutput(ITestOutputHelper output)
    {
        this.output = output;
    }

    public void WriteLine(string line)
    {
        this.output.WriteLine(line);
    }
}
