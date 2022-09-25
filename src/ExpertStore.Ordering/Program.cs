using ExpertStore.Ordering.Domain;
using ExpertStore.Ordering.Repositories;
using ExpertStore.Ordering.Subscribers;
using ExpertStore.Ordering.Tracing;
using ExpertStore.Ordering.UseCases;
using ExpertStore.SeedWork.Interfaces;
using ExpertStore.SeedWork.RabbitProducer;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddOpenTelemetry(options =>
    {
        options.AddConsoleExporter();
    });
});

builder.Services.AddSingleton<IOrderRepository, OrderRepository>();

builder.Services.AddTransient<IUseCase<CreateOrderInput, CreateOrderOutput>, CreateOrder>();
builder.Services.AddTransient<IUseCase<List<ListOrdersOutputItem>>, ListOrders>();
builder.Services.AddTransient<IUpdateOrderPaymentResult, UpdateOrderPaymentResult>();
builder.Services.AddRabbitMessageBus();
builder.Services.AddPaymentSubscriber();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOpenTelemetryTracing(traceProvider =>
{
    traceProvider
        .AddSource(OpenTelemetryExtensions.ServiceName)
        .SetResourceBuilder(
            ResourceBuilder.CreateDefault()
                .AddService(OpenTelemetryExtensions.ServiceName,
                    serviceVersion: OpenTelemetryExtensions.ServiceVersion))
        .AddAspNetCoreInstrumentation()
        .SetSampler(new AlwaysOnSampler())
        /* .AddJaegerExporter(exporter =>
        {
            exporter.AgentHost = builder.Configuration["OpenTelemetry:AgentHost"];
            exporter.AgentPort = Convert.ToInt32(builder.Configuration["OpenTelemetry:AgentPort"]); 
        })*/
        .AddOtlpExporter(exporter =>
        {
            exporter.Endpoint = new Uri(builder.Configuration["OpenTelemetry:Endpoint"]);
            exporter.Protocol = OtlpExportProtocol.HttpProtobuf;
        })
        .AddConsoleExporter();
});

builder.Logging.ClearProviders();

builder.Logging.AddOpenTelemetry(options =>
{
    options.SetResourceBuilder(
        ResourceBuilder.CreateDefault()
            .AddService(OpenTelemetryExtensions.ServiceName,
                serviceVersion: OpenTelemetryExtensions.ServiceVersion));

    var logExporter = "otlp";
    switch (logExporter)
    {
        case "otlp":
            options.AddOtlpExporter(exporter =>
            {
                exporter.Endpoint = new Uri(builder.Configuration["OpenTelemetry:Endpoint"]);
                exporter.Protocol = OtlpExportProtocol.HttpProtobuf;
            });
            break;
        default:
            options.AddConsoleExporter();
            break;
    }
});

builder.Services.Configure<OpenTelemetryLoggerOptions>(opt =>
{
    opt.IncludeScopes = true;
    opt.ParseStateValues = true;
    opt.IncludeFormattedMessage = true;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();