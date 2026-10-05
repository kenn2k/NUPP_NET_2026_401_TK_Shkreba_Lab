namespace OnlineStore.Domain;

public sealed class Customer
{
    public Customer(string fullName, string email)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("Customer name is required.", nameof(fullName));
        }

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
        {
            throw new ArgumentException("A valid email is required.", nameof(email));
        }

        Id = Guid.NewGuid();
        FullName = fullName;
        Email = email;
    }

    public Guid Id { get; }
    public string FullName { get; }
    public string Email { get; }
}
