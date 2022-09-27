namespace ExpertStore.Ordering.Domain;

public class Order
{
    public Order(Person retailer, Person shopper)
    {
        Id = Guid.NewGuid();
        Status = OrderStatus.Processing;
        Date = DateTime.Now;
        Delivery = null;

        _items = new List<OrderItem>();

        Retailer = retailer;
        Shopper = shopper;
    }

    public void AddItem(int productId, int quantity, decimal value)
        => _items.Add(new OrderItem(productId, quantity, value));

    public void UpdatePaymentStatus(OrderStatus newStatus)
        => Status = newStatus;

    public void RegisterDelivery(Delivery delivery)
        => Delivery = delivery;

    public void UpdateDeliveryStatus(DeliveryStatus newStatus)
        => Delivery?.UpdateStatus(newStatus);

    public Guid Id { get; }
    public OrderStatus Status { get; private set; }
    public DateTime Date { get; }
    public Person Retailer { get; }
    public Person Shopper { get; }

    public Delivery? Delivery { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    readonly List<OrderItem> _items;
}