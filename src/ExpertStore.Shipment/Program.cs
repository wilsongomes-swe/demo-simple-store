using ExpertStore.SeedWork.Interfaces;
using ExpertStore.SeedWork.RabbitProducer;
using ExpertStore.Shipment.Application;
using ExpertStore.Shipment.Application.Integration;
using ExpertStore.Shipment.Configuration;
using ExpertStore.Shipment.Domain;
using ExpertStore.Shipment.Infra;
using ExpertStore.Shipment.Subscribers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IUseCase<RegisterShipmentInput, RegisterShipmentOutput>, RegisterShipment>();
builder.Services.AddTransient<IUseCase<IReadOnlyCollection<ListShipmentsItem>?>, ListShipments>();
builder.Services.AddTransient<IUseCase<GetShipmentDetailInput, ShipmentDetail?>, GetShipmentDetail>();
builder.Services.AddSingleton<IShipmentRepository, ShipmentRepository>();
builder.Services.AddTransient<IOrderingService, OrderingService>();
builder.Services.AddTransient<ICarrierService, CarrierService>();
builder.Services.AddRabbitMessageBus();
builder.Services.AddHostedService<PaymentSubscriber>();
builder.Services.ConfigureFlur();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();
