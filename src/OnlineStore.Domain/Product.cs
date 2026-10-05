namespace OnlineStore.Domain;

public sealed class Product
{
    public Product(string name, decimal price, string category)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Product name is required.", nameof(name));
        }

        if (price <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price), "Product price must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(category))
        {
            throw new ArgumentException("Product category is required.", nameof(category));
        }

        Id = Guid.NewGuid();
        Name = name;
        Price = price;
        Category = category;
    }

    public Guid Id { get; }
    public string Name { get; }
    public decimal Price { get; }
    public string Category { get; }
}
