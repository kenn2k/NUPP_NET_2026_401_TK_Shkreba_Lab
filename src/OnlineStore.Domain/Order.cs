namespace OnlineStore.Domain;

public sealed class Order
{
    private readonly List<OrderItem> _items = [];

    public Order(Customer customer)
    {
        ArgumentNullException.ThrowIfNull(customer);
        customer.Validate();

        Customer = customer;
        Id = Guid.NewGuid();
        Status = OrderStatus.Created;
    }

    public Guid Id { get; }
    public Customer Customer { get; }
    public OrderStatus Status { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public decimal Total => _items.Sum(item => item.Subtotal);
    public event EventHandler<Order>? OrderPaid;

    public void AddItem(Product product, int quantity, Action<string>? logger = null)
    {
        EnsureCreated();

        ArgumentNullException.ThrowIfNull(product);

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
        }

        var existingItem = _items.SingleOrDefault(item => item.Product.Id == product.Id);
        if (existingItem is not null)
        {
            existingItem.IncreaseQuantity(quantity);
            logger?.Invoke($"Increased quantity of {product.Name} by {quantity}.");
            return;
        }

        _items.Add(new OrderItem(product, quantity));
        logger?.Invoke($"Added {product.Name} x{quantity} to order {Id}.");
    }

    public void RemoveItem(Guid productId, Action<string>? logger = null)
    {
        EnsureCreated();

        var item = _items.SingleOrDefault(orderItem => orderItem.Product.Id == productId)
            ?? throw new InvalidOperationException("The product is not in this order.");

        _items.Remove(item);
        logger?.Invoke($"Removed {item.Product.Name} from order {Id}.");
    }

    public PaymentReceipt Pay(IPaymentMethod paymentMethod, Action<string>? logger = null)
    {
        EnsureCreated();

        if (_items.Count == 0)
        {
            throw new InvalidOperationException("An empty order cannot be paid.");
        }

        ArgumentNullException.ThrowIfNull(paymentMethod);

        var receipt = paymentMethod.Pay(Total);
        Status = OrderStatus.Paid;
        logger?.Invoke($"Paid order {Id} for {Total:F2} UAH using {paymentMethod.Name}.");
        OrderPaid?.Invoke(this, this);
        return receipt;
    }

    public void Ship()
    {
        if (Status != OrderStatus.Paid)
        {
            throw new InvalidOperationException("Only a paid order can be shipped.");
        }

        Status = OrderStatus.Shipped;
    }

    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
        {
            throw new InvalidOperationException("Only a shipped order can be delivered.");
        }

        Status = OrderStatus.Delivered;
    }

    public void Cancel()
    {
        EnsureCreated();
        Status = OrderStatus.Cancelled;
    }

    public string GetStatusDescription() => Status switch
    {
        OrderStatus.Created => "Created",
        OrderStatus.Paid => "Paid",
        OrderStatus.Shipped => "Shipped",
        OrderStatus.Delivered => "Delivered",
        OrderStatus.Cancelled => "Cancelled",
        _ => "Unknown"
    };

    public string GetOrderCategory() => Total switch
    {
        < 500m => "Small",
        < 5000m => "Medium",
        _ => "Large"
    };

    public string GetDeliveryOption() => this switch
    {
        { Status: OrderStatus.Paid, Total: >= 2000m } => "Free Delivery",
        _ => "Standard Delivery"
    };

    private void EnsureCreated()
    {
        if (Status != OrderStatus.Created)
        {
            throw new InvalidOperationException($"An order with status {Status} cannot be changed or paid.");
        }
    }
}
