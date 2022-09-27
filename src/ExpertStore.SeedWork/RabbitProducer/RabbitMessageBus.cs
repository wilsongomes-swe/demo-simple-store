using ExpertStore.SeedWork.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RabbitMQ.Client;
using System.Text;

namespace ExpertStore.SeedWork.RabbitProducer;

public class RabbitMessageBus : IEventBus
{
    public RabbitMessageBus(IRabbitConnection connection, IConfiguration config, ILogger<RabbitMessageBus> logger)
    {
        _connection = connection.Connection;
        _exchange = config.GetSection("RabbitMQ")["Exchange"];
        _logger = logger;
        SetupExchange();
    }

    public void Publish(IIntegrationEvent @event)
    {
        var channel = _connection.CreateModel();
        var payload = JsonConvert.SerializeObject(@event, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
        channel.BasicPublish(
            _exchange,
            @event.GetType().Name.ToSnakeDotCase(),
            null,
            Encoding.UTF8.GetBytes(payload)
        );
        _logger.LogInformation($"Event published: {@event.GetType().Name.ToSnakeDotCase()}");
    }

    void SetupExchange()
    {
        var channel = _connection.CreateModel();
        channel.ExchangeDeclare(
            _exchange,
            ExchangeType.Topic,
            true
        );
        _logger.LogInformation("Exchange declared");
    }

    readonly IConnection _connection;
    readonly ILogger<RabbitMessageBus> _logger;
    readonly string _exchange;
}

public static class RabbitMessageBusExtensions
{
    public static IServiceCollection AddRabbitMessageBus(this IServiceCollection services)
    {
        services.AddSingleton<IRabbitConnection, RabbitConnection>();
        services.AddSingleton<IEventBus, RabbitMessageBus>();

        return services;
    }
}

static class StringExtensions
{
    public static string ToSnakeDotCase(this string text)
    {
        if (text == null) throw new ArgumentNullException(nameof(text));
        if (text.Length < 2) return text;
        var sb = new StringBuilder();
        sb.Append(char.ToLowerInvariant(text[0]));
        for (var i = 1; i < text.Length; ++i)
        {
            var c = text[i];
            if (char.IsUpper(c))
            {
                sb.Append('.');
                sb.Append(char.ToLowerInvariant(c));
            }
            else
            {
                sb.Append(c);
            }
        }
        var eventName = sb.ToString();
        eventName = eventName.Replace(".event", "");
        return eventName;
    }
}