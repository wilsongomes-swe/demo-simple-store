namespace ExpertStore.Ordering.Domain;

public class OrderItem
{
    public int ProductId { get; }
    public int Quantity { get; }
    public decimal Value { get; }

    public OrderItem(int productId, int quantity, decimal value)
    {
        ProductId = productId;
        Quantity = quantity;
        Value = value;
    }
}
