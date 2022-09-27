using ExpertStore.Ordering.Tracing;
using ExpertStore.SeedWork.Interfaces;
using ExpertStore.SeedWork.RabbitProducer;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PaymentsProcessor;
using PaymentsProcessor.UseCases;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUseCase<ProcessPaymentInput, ProcessPaymentOutput>, ProcessPayment>();
builder.Services.AddRabbitMessageBus();
builder.Services.AddPaymentProcessorSubscriber();

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
        .AddJaegerExporter(exporter =>
        {
            exporter.AgentHost = builder.Configuration["OpenTelemetry:AgentHost"];
            exporter.AgentPort = Convert.ToInt32(builder.Configuration["OpenTelemetry:AgentPort"]);
        });
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.Run();