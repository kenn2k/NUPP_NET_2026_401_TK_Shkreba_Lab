namespace OnlineStore.Domain;

public sealed class Order
{
    private readonly List<OrderItem> _items = [];

    public Order(Customer customer)
    {
        Customer = customer ?? throw new ArgumentNullException(nameof(customer));
        Id = Guid.NewGuid();
        Status = OrderStatus.Draft;
    }

    public Guid Id { get; }
    public Customer Customer { get; }
    public OrderStatus Status { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public decimal Total => _items.Sum(item => item.Subtotal);

    public void AddItem(Product product, int quantity)
    {
        EnsureDraft();

        if (product is null)
        {
            throw new ArgumentNullException(nameof(product));
        }

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
        }

        var existingItem = _items.SingleOrDefault(item => item.Product.Id == product.Id);
        if (existingItem is not null)
        {
            existingItem.IncreaseQuantity(quantity);
            return;
        }

        _items.Add(new OrderItem(product, quantity));
    }

    public void RemoveItem(Guid productId)
    {
        EnsureDraft();

        var item = _items.SingleOrDefault(orderItem => orderItem.Product.Id == productId)
            ?? throw new InvalidOperationException("The product is not in this order.");

        _items.Remove(item);
    }

    public PaymentReceipt Pay(IPaymentMethod paymentMethod)
    {
        EnsureDraft();

        if (_items.Count == 0)
        {
            throw new InvalidOperationException("An empty order cannot be paid.");
        }

        ArgumentNullException.ThrowIfNull(paymentMethod);

        var receipt = paymentMethod.Pay(Total);
        Status = OrderStatus.Paid;
        return receipt;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Paid)
        {
            throw new InvalidOperationException("A paid order cannot be cancelled.");
        }

        if (Status == OrderStatus.Cancelled)
        {
            throw new InvalidOperationException("The order is already cancelled.");
        }

        Status = OrderStatus.Cancelled;
    }

    private void EnsureDraft()
    {
        if (Status == OrderStatus.Paid)
        {
            throw new InvalidOperationException("A paid order cannot be changed or paid again.");
        }

        if (Status == OrderStatus.Cancelled)
        {
            throw new InvalidOperationException("A cancelled order cannot be changed or paid.");
        }
    }
}
