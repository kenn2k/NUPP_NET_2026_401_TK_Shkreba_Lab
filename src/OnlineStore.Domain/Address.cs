namespace OnlineStore.Domain;

/// <summary>
/// Immutable delivery address represented by its value.
/// </summary>
public sealed record Address(string City, string Street, string Building, string PostalCode);
