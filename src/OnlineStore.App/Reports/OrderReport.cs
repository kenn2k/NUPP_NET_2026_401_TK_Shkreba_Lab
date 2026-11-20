namespace OnlineStore.App.Reports;

public sealed record OrderReport(
    Guid OrderId,
    string CustomerName,
    decimal Total,
    string Status,
    int ItemsCount);
