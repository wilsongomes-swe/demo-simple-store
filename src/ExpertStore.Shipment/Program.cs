using ExpertStore.SeedWork.Interfaces;
using ExpertStore.Shipment.Application;
using ExpertStore.Shipment.Application.Integration;
using ExpertStore.Shipment.Domain;
using ExpertStore.Shipment.Infra;

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

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
