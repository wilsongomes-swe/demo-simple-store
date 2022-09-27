namespace ExpertStore.Ordering.Domain;

public class Delivery
{
    public Delivery(string carrierReference, DeliveryStatus currentStatus)
    {
        CurrentStatus = currentStatus;
    }

    public void UpdateStatus(DeliveryStatus newStatus)
        => CurrentStatus = newStatus;

    public DeliveryStatus CurrentStatus { get; private set; }
}