namespace OnlineStore.Domain;

public static class OrderExtensions
{
    public static bool IsLargeOrder(this Order order)
    {
        ArgumentNullException.ThrowIfNull(order);
        return order.Total >= 5000m;
    }

    public static string GetSummary(this Order order)
    {
        ArgumentNullException.ThrowIfNull(order);
        return $"Order {order.Id}: {order.Customer.Name}, {order.Items.Count} item types, {order.Total:F2} UAH, {order.Status}.";
    }
}
