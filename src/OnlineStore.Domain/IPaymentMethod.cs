namespace OnlineStore.Domain;

public interface IPaymentMethod
{
    string Name { get; }

    PaymentReceipt Pay(decimal amount);
}
