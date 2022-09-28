using ExpertStore.SeedWork.IntegrationEvents;
using ExpertStore.SeedWork.Interfaces;
using ExpertStore.SeedWork.RabbitProducer;
using ExpertStore.Payment.UseCases;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ExpertStore.Payment;

public class PaymentProcessorSubscriber : BackgroundService
{
    public PaymentProcessorSubscriber(
        ILogger<PaymentProcessorSubscriber> logger,
        IRabbitConnection rabbitConnection,
        IConfiguration config,
        IServiceScopeFactory factory
    )
    {
        _logger = logger;
        _processPaymentUseCase =
            factory.CreateScope().ServiceProvider.GetRequiredService<IUseCase<ProcessPaymentInput, ProcessPaymentOutput>>();
        _rabbitConnection = rabbitConnection;

        Queue = config.GetSection("RabbitMQ").GetValue<string>("PaymentProcessorSubscriberQueue");
        Exchange = config.GetSection("RabbitMQ").GetValue<string>("Exchange");

        _channel = _rabbitConnection.Connection.CreateModel();
        _channel.ExchangeDeclare(
            Exchange,
            ExchangeType.Topic,
            true
        );

        _channel.QueueDeclare(Queue, false, false, false, null);
        _channel.QueueBind(Queue, Exchange, "order.created");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (sender, eventArgs) =>
        {
            var contentArray = eventArgs.Body.ToArray();
            var contentString = Encoding.UTF8.GetString(contentArray);
            var message = JsonConvert.DeserializeObject<OrderCreatedEvent>(contentString);
            if (message == null) throw new NullReferenceException("Message received is null");

            _logger.LogInformation($"Payment processor received an event: {contentString}");

            var output = await _processPaymentUseCase
                .Handle(new ProcessPaymentInput(message.OrderId, message.TotalValue));

            _channel.BasicAck(eventArgs.DeliveryTag, false);

            _logger.LogInformation($"Payment processor: {message.OrderId} -> {(output.IsApproved ? "Approved" : "Refused")}");
        };

        _channel.BasicConsume(Queue, false, consumer);

        return Task.CompletedTask;
    }

    readonly string Queue;
    readonly string Exchange;
    readonly IModel _channel;
    readonly ILogger _logger;
    readonly IUseCase<ProcessPaymentInput, ProcessPaymentOutput> _processPaymentUseCase;
    readonly IRabbitConnection _rabbitConnection;
}

public static class PaymentProcessorSubscriberExtensions
{
    public static IServiceCollection AddPaymentProcessorSubscriber(this IServiceCollection services)
    {
        services.AddHostedService<PaymentProcessorSubscriber>();
        return services;
    }
}