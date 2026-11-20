namespace OnlineStore.Domain;

/// <summary>
/// Represents a product that can be sold by an online store.
/// </summary>
public sealed record Product(string Name, decimal Price, string Category);
