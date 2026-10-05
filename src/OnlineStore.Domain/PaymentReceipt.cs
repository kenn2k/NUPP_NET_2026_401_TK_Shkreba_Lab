namespace OnlineStore.Domain;

public sealed record PaymentReceipt(string PaymentMethod, decimal Amount, DateTimeOffset PaidAt);
