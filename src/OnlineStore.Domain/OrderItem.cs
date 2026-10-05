namespace OnlineStore.Domain;

public sealed class OrderItem
{
    internal OrderItem(Product product, int quantity)
    {
        Product = product ?? throw new ArgumentNullException(nameof(product));

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
        }

        Quantity = quantity;
    }

    public Product Product { get; }
    public int Quantity { get; private set; }
    public decimal Subtotal => Product.Price * Quantity;

    internal void IncreaseQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
        }

        Quantity += quantity;
    }
}
