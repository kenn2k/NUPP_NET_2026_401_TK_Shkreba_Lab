namespace OnlineStore.Domain;

public sealed class CardPayment : IPaymentMethod
{
    public CardPayment(string lastFourDigits)
    {
        if (lastFourDigits is null || lastFourDigits.Length != 4 || !lastFourDigits.All(char.IsDigit))
        {
            throw new ArgumentException("Card number must contain exactly four final digits.", nameof(lastFourDigits));
        }

        LastFourDigits = lastFourDigits;
    }

    public string LastFourDigits { get; }
    public string Name => $"Card ending in {LastFourDigits}";

    public PaymentReceipt Pay(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Payment amount must be greater than zero.");
        }

        return new PaymentReceipt(Name, amount, DateTimeOffset.UtcNow);
    }
}
