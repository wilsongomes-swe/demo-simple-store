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

public class ShipmentContractTests : IClassFixture<ApiFixture>
{
    private readonly ITestOutputHelper _output;
    private readonly ApiFixture _api;

    public ShipmentContractTests(ApiFixture api, ITestOutputHelper output)
    {
        _output = output;
        _api = api;
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

        verifier.ServiceProvider("ExpertStore-Ordering", new Uri("http://localhost:5251/"))
                .WithFileSource(new FileInfo(Path.Combine("..", "..", "..", "..", "..", "pacts", "ExpertStore-Shipment-ExpertStore-Ordering.json")))
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

public class ApiFixture : IDisposable
{
    private readonly IHost server;

    public Uri ServerUri { get; }

    public ApiFixture()
    {
        this.ServerUri = new Uri("http://localhost:9222");

        this.server = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder => { 
                webBuilder.UseUrls(this.ServerUri.ToString());

                // webBuilder.UseStartup<Program>();
            })
            .Build();
        this.server.Start();
    }

    public void Dispose()
    {
        this.server.Dispose();
    }
}

//public class TestStartup
//{
//    private readonly Startup inner;

//    public TestStartup(IConfiguration configuration)
//    {
//        this.inner = new Startup(configuration);
//    }

//    public void ConfigureServices(IServiceCollection services)
//    {
//        this.inner.ConfigureServices(services);
//    }

//    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
//    {
//        app.UseMiddleware<ProviderStateMiddleware>()
//           .UseMiddleware<AuthorizationTokenReplacementMiddleware>();

//        this.inner.Configure(app, env);
//    }
//}