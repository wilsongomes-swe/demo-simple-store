using ExpertStore.SeedWork.Interfaces;

namespace ExpertStore.SeedWork.IntegrationEvents;

public record PaymentApprovedEvent : PaymentProcessedEvent
{
    public PaymentApprovedEvent(Guid orderId)
        : base(orderId, true, "")
    {
    }
}

public record PaymentRefusedEvent : PaymentProcessedEvent
{
    public PaymentRefusedEvent(Guid orderId, string note)
        : base(orderId, false, note)
    {
    }
}

public record PaymentProcessedEvent(Guid OrderId, bool Approved, string Note) : IIntegrationEvent;