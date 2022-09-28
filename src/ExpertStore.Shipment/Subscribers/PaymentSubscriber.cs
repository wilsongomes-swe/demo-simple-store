using ExpertStore.SeedWork.IntegrationEvents;
using ExpertStore.SeedWork.Interfaces;
using ExpertStore.SeedWork.RabbitProducer;
using ExpertStore.Shipment.Application;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ExpertStore.Shipment.Subscribers;

public class PaymentSubscriber : BackgroundService
{
    public PaymentSubscriber(
        ILogger<PaymentSubscriber> logger,
        IRabbitConnection rabbitConnection,
        IConfiguration config,
        IServiceScopeFactory factory
    )
    {
        _logger = logger;
        _registerShipmentUseCase = factory.CreateScope().ServiceProvider.GetRequiredService<IUseCase<RegisterShipmentInput, RegisterShipmentOutput>>();
        _rabbitConnection = rabbitConnection;

        Queue = config.GetSection("RabbitMQ").GetValue<string>("PaymentSubscriberQueue");
        Exchange = config.GetSection("RabbitMQ").GetValue<string>("Exchange");

        _channel = _rabbitConnection.Connection.CreateModel();
        _channel.ExchangeDeclare(
            Exchange,
            ExchangeType.Topic,
            true,
            false
        );

        _channel.QueueDeclare(Queue, false, false, false, null);
        _channel.QueueBind(Queue, Exchange, "payment.approved");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (sender, eventArgs) =>
        {
            var contentArray = eventArgs.Body.ToArray();
            var contentString = Encoding.UTF8.GetString(contentArray);
            var message = JsonConvert.DeserializeObject<PaymentProcessedEvent>(contentString);
            if (message is null) throw new NullReferenceException("Message received is null");

            _logger.LogInformation($"Shipment payment subscriber received an event: {contentString}");

            var output = await _registerShipmentUseCase.Handle(new RegisterShipmentInput(message.OrderId));

            _logger.LogInformation($"Shipment payment subscriber finished, shipment cretaed...");

            _channel.BasicAck(eventArgs.DeliveryTag, false);
        };

        _channel.BasicConsume(Queue, false, consumer);

        return Task.CompletedTask;
    }

    readonly IModel _channel;
    readonly ILogger _logger;
    readonly IUseCase<RegisterShipmentInput, RegisterShipmentOutput> _registerShipmentUseCase;
    readonly IRabbitConnection _rabbitConnection;

    readonly string Queue;
    readonly string Exchange;
}

public static class PaymentSubscriberExtensions
{
    public static IServiceCollection AddPaymentSubscriber(this IServiceCollection services)
    {
        services.AddHostedService<PaymentSubscriber>();
        return services;
    }
}