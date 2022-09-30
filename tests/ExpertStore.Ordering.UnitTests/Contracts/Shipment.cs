using PactNet.Verifier;
using PactNet;

namespace ExpertStore.Ordering.UnitTests.Contracts;

public class Shipment
{
    [Fact]
    public void VerifyLatestPacts()
    {
        var config = new PactVerifierConfig
        {
            LogLevel = PactLogLevel.Information,
        };

        IPactVerifier verifier = new PactVerifier(config);

        verifier.ServiceProvider("My Provider", new Uri(""))
                .WithFileSource(new FileInfo(Path.Combine("..", "..", "..", "..", "..", "pacts")))
                .Verify();
    }
}
