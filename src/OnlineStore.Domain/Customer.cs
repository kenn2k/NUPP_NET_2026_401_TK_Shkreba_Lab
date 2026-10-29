namespace OnlineStore.Domain;

public sealed class Customer
{
    public Guid Id { get; } = Guid.NewGuid();

    public required string Name { get; init; }

    public required string Email { get; init; }

    public required Address Address { get; init; }

    public string? PhoneNumber { get; init; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new InvalidOperationException("Customer name is required.");
        }

        if (string.IsNullOrWhiteSpace(Email) || !Email.Contains('@'))
        {
            throw new InvalidOperationException("A valid email is required.");
        }
    }
}
