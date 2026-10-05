namespace OnlineStore.Domain;

public sealed class BankTransferPayment : IPaymentMethod
{
    public BankTransferPayment(string iban)
    {
        if (string.IsNullOrWhiteSpace(iban) || iban.Length < 10)
        {
            throw new ArgumentException("A valid bank account identifier is required.", nameof(iban));
        }

        Iban = iban;
    }

    public string Iban { get; }
    public string Name => "Bank transfer";

    public PaymentReceipt Pay(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Payment amount must be greater than zero.");
        }

        return new PaymentReceipt(Name, amount, DateTimeOffset.UtcNow);
    }
}
