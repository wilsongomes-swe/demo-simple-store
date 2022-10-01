using ExpertStore.Ordering.Domain;
using ExpertStore.Ordering.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PactNet;
using PactNet.Infrastructure.Outputters;
using PactNet.Verifier;
using Xunit.Abstractions;

namespace ExpertStore.Ordering.UnitTests.Contracts;

public class ShipmentContractTests : IClassFixture<WebApplicationFactoryWithRealServer<Program>>
{
    private readonly ITestOutputHelper _output;
    private readonly WebApplicationFactoryWithRealServer<Program> _api;

    public ShipmentContractTests(WebApplicationFactoryWithRealServer<Program> api, ITestOutputHelper output)
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

        verifier.ServiceProvider("ExpertStore-Ordering", _api.Uri /* new Uri("http://localhost:5251") */)
                .WithFileSource(new FileInfo(Path.Combine("..", "..", "..", "..", "..", "pacts", "ExpertStore-Shipment-ExpertStore-Ordering.json")))
                //.WithFilter(providerState: "There is not an order with the specified ID")
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


public class WebApplicationFactoryWithRealServer<TStartup>
    : WebApplicationFactory<TStartup>
    where TStartup : class
{
    private IHost? _host;
    public Uri? Uri;

    public WebApplicationFactoryWithRealServer()
        : base()
    {
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var repository = new OrderRepository();
            repository.Save(new Order(
                new Guid("d0389a71-e1c5-4667-b068-7466c12c433a"),
                OrderStatus.Approved,
                DateTime.Parse("2022-09-29T16:58:21.833707+01:00"),
                new(1, "Nike", "NY - NY US 10892"),
                new(2, "Wilson", "SP - SP BR 75880-000"),
                null,
                new List<OrderItem>()
                {
                    new(1, 10, (decimal)200.0),
                    new(2, 1, (decimal)200.0)
                }
            ));
            services.RemoveAll<IOrderRepository>();
            services.AddSingleton<IOrderRepository>(repository);
        });

        base.ConfigureWebHost(builder);
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var testHost = builder.Build();
        builder.ConfigureWebHost(webHostBuilder => webHostBuilder.UseKestrel(cfg => {
            cfg.AllowSynchronousIO = true;
            cfg.ListenLocalhost(/* Random.Shared.Next(50_000, 60_000) */ 51_102);
        }));
        //builder.ConfigureServices(services =>
        //{
        //    // var serviceProvider = services.BuildServiceProvider();
        //    // using var scope = serviceProvider.CreateScope();
        //    var repository = new OrderRepository();
        //    repository.Save(new Order(
        //        new Guid("d0389a71-e1c5-4667-b068-7466c12c433a"),
        //        OrderStatus.Approved,
        //        DateTime.Parse("2022-09-29T16:58:21.833707+01:00"),
        //        new(1, "Nike", "2022-09-29T16:58:21.833707+01:00"),
        //        new(2, "Wilson", "SP - SP BR 75880-000"),
        //        null,
        //        new List<OrderItem>()
        //        {
        //            new(1, 10, 200),
        //            new(2, 1, 200)
        //        }
        //    ));
        //    services.RemoveAll<IOrderRepository>();
        //    services.AddSingleton<IOrderRepository>(repository);
        //});
        _host = builder.Build();
        _host.Start();
        var server = _host.Services.GetRequiredService<IServer>();
        var addresses = server.Features.Get<IServerAddressesFeature>();
        ClientOptions.BaseAddress = addresses!.Addresses.Select(x => new Uri(x)).Last();
        Uri = ClientOptions.BaseAddress;
        testHost.Start();
        return testHost;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _host?.Dispose();
    }
}
