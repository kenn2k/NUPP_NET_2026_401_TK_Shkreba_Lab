using OnlineStore.Domain;

namespace OnlineStore.App.Services;

public sealed class NotificationService
{
    public void HandleOrderPaid(object? sender, Order order)
    {
        Console.WriteLine($"Notification: payment for order {order.Id} was received for {order.Customer.Name}.");
    }
}
