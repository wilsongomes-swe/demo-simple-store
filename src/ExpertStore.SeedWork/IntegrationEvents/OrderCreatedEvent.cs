using ExpertStore.SeedWork.Interfaces;

namespace ExpertStore.SeedWork.IntegrationEvents;

public record OrderCreatedEvent(Guid OrderId, DateTime Date, decimal TotalValue) : IIntegrationEvent;