using ExpertStore.SeedWork.IntegrationEvents;
using ExpertStore.SeedWork.Interfaces;

namespace ExpertStore.Payment.UseCases;

public class ProcessPayment : IUseCase<ProcessPaymentInput, ProcessPaymentOutput>
{
    public ProcessPayment(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public Task<ProcessPaymentOutput> Handle(ProcessPaymentInput input)
    {
        Thread.Sleep(10 * 1000);
        var approved = input.TotalValue < 500;

        if (approved)
            _eventBus.Publish(new PaymentApprovedEvent(input.OrderId));
        else
            _eventBus.Publish(new PaymentRefusedEvent(input.OrderId, "Credid Card Refused"));

        return Task.FromResult(new ProcessPaymentOutput(input.OrderId, approved));
    }

    readonly IEventBus _eventBus;
}

public record ProcessPaymentInput(Guid OrderId, decimal TotalValue);

public record ProcessPaymentOutput(Guid OrderId, bool IsApproved);