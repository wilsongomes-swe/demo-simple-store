using ExpertStore.SeedWork.Interfaces;
using ExpertStore.Shipment.Application;
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
