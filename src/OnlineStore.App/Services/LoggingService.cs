using OnlineStore.Domain;

namespace OnlineStore.App.Services;

public sealed class LoggingService
{
    public void HandleOrderPaid(object? sender, Order order)
    {
        Console.WriteLine($"Event log: OrderPaid raised for {order.Id}; status is {order.Status}.");
    }
}
