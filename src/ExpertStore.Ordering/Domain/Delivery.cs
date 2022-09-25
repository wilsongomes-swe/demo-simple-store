namespace ExpertStore.Ordering.Domain;

public class Delivery
{
    public string CarrierReference { get; private set; }
    public DeliveryStatus CurrentStatus { get; private set; }

    public Delivery(string carrierReference, DeliveryStatus currentStatus)
    {
        CarrierReference = carrierReference;
        CurrentStatus = currentStatus;  
    }

    public void UpdateStatus(DeliveryStatus newStatus) 
        => CurrentStatus = newStatus;
}