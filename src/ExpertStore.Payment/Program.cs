using ExpertStore.SeedWork.Interfaces;
using ExpertStore.SeedWork.RabbitProducer;
using ExpertStore.Payment;
using ExpertStore.Payment.UseCases;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUseCase<ProcessPaymentInput, ProcessPaymentOutput>, ProcessPayment>();
builder.Services.AddRabbitMessageBus();
builder.Services.AddPaymentProcessorSubscriber();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.Run();