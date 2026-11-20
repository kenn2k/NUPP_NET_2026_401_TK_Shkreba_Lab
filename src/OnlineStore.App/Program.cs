using OnlineStore.Domain;
using OnlineStore.App.Services;
using OnlineStore.App.Reports;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Net.Http.Json;
using System.Diagnostics;
using OnlineStore.App.External;

Console.WriteLine("OnlineStore — Laboratory Work 8");
Console.WriteLine("Student: Anton Shkreba | Group: 301 TK | Variant: 1");
Console.WriteLine();

var customers = new[]
{
    CreateCustomer("Anton Shkreba", "anton.shkreba@student.example", "Kyiv"),
    CreateCustomer("Iryna Bondar", "iryna.bondar@example.com", "Lviv"),
    CreateCustomer("Olena Koval", "olena.koval@example.com", "Odesa"),
    CreateCustomer("Dmytro Melnyk", "dmytro.melnyk@example.com", "Dnipro"),
    CreateCustomer("Taras Shevchenko", "taras.shevchenko@example.com", "Kharkiv")
};

var mouse = new Product("Wireless Mouse", 799m, "Accessories");
var keyboard = new Product("Mechanical Keyboard", 2199m, "Accessories");
var headset = new Product("Gaming Headset", 1599m, "Accessories");
var webcam = new Product("Full HD Webcam", 2499m, "Electronics");
var monitor = new Product("4K Monitor", 12499m, "Electronics");
var usbCable = new Product("USB-C Cable", 299m, "Accessories");

var repository = new InMemoryRepository<Order>();
var paymentMethod = new CardPayment("4242");

foreach (var order in new[]
         {
             CreatePaidOrder(customers[0], paymentMethod, (mouse, 2), (keyboard, 1)),
             CreatePaidOrder(customers[1], paymentMethod, (monitor, 1)),
             CreatePaidOrder(customers[2], paymentMethod, (webcam, 1), (headset, 1)),
             CreatePaidOrder(customers[3], paymentMethod, (keyboard, 2)),
             CreatePaidOrder(customers[4], paymentMethod, (mouse, 4)),
             CreatePaidOrder(customers[0], paymentMethod, (usbCable, 5)),
             CreatePaidOrder(customers[1], paymentMethod, (headset, 2)),
             CreatePaidOrder(customers[2], paymentMethod, (keyboard, 1), (mouse, 1)),
             CreatePaidOrder(customers[3], paymentMethod, (webcam, 2)),
             CreatePaidOrder(customers[4], paymentMethod, (usbCable, 8)),
             CreatePaidOrder(customers[0], paymentMethod, (monitor, 1)),
             CreatePaidOrder(customers[1], paymentMethod, (keyboard, 1)),
             CreatePaidOrder(customers[2], paymentMethod, (mouse, 3)),
             CreatePaidOrder(customers[3], paymentMethod, (headset, 3)),
             CreatePaidOrder(customers[4], paymentMethod, (webcam, 1), (usbCable, 1)),
             CreatePaidOrder(customers[0], paymentMethod, (keyboard, 1), (headset, 1)),
             CreatePaidOrder(customers[1], paymentMethod, (mouse, 2), (usbCable, 3)),
             CreatePaidOrder(customers[2], paymentMethod, (monitor, 2))
         })
{
    repository.Add(order);
}

IEnumerable<Order> orders = repository.GetAll();

var ordersOver1000 = orders
    .Where(order => order.Total > 1000m)
    .OrderByDescending(order => order.Total)
    .ToList();

var topFiveOrders = orders
    .OrderByDescending(order => order.Total)
    .Take(5)
    .ToList();

var totalSales = orders.Sum(order => order.Total);
var averageOrderTotal = orders.Average(order => order.Total);

var orderCountByCustomer = orders
    .GroupBy(order => order.Customer.Id)
    .Select(group => new
    {
        Customer = group.First().Customer.Name,
        Count = group.Count()
    })
    .OrderBy(result => result.Customer)
    .ToList();

var productsFromOrders = orders
    .SelectMany(order => order.Items)
    .Select(item => item.Product)
    .DistinctBy(product => product.Id)
    .OrderBy(product => product.Name)
    .ToList();

var topThreeProducts = orders
    .SelectMany(order => order.Items)
    .GroupBy(item => item.Product.Id)
    .Select(group => new
    {
        Product = group.First().Product.Name,
        Quantity = group.Sum(item => item.Quantity)
    })
    .OrderByDescending(result => result.Quantity)
    .Take(3)
    .ToList();

var highSpendingCustomers = orders
    .GroupBy(order => order.Customer.Id)
    .Select(group => new
    {
        Customer = group.First().Customer.Name,
        Spending = group.Sum(order => order.Total)
    })
    .Where(result => result.Spending > 5000m)
    .OrderByDescending(result => result.Spending)
    .ToList();

var hasOrderOver10000 = orders.Any(order => order.Total > 10000m);

Dictionary<Guid, decimal> spendingByCustomer = orders
    .GroupBy(order => order.Customer.Id)
    .ToDictionary(group => group.Key, group => group.Sum(order => order.Total));

PrintHeading("Laboratory Work 5: delegates, events and extensions");

Action<string> logger = message => Console.WriteLine($"[LOG {DateTime.Now:HH:mm:ss}] {message}");
var notificationService = new NotificationService();
var loggingService = new LoggingService();
var eventOrder = new Order(customers[0]);

eventOrder.OrderPaid += notificationService.HandleOrderPaid;
eventOrder.OrderPaid += loggingService.HandleOrderPaid;

eventOrder.AddItem(mouse, 1, logger);
eventOrder.AddItem(keyboard, 1, logger);
eventOrder.RemoveItem(mouse.Id, logger);
eventOrder.Pay(new CardPayment("1111"), logger);

var discountPolicies = new Dictionary<string, Func<decimal, decimal>>
{
    ["No discount"] = total => total,
    ["10% discount"] = total => total * 0.90m,
    ["20% discount"] = total => total * 0.80m
};

Console.WriteLine("Discount policies for the event order:");
foreach (var (name, policy) in discountPolicies)
{
    Console.WriteLine($"{name}: {policy(eventOrder.Total):F2} UAH");
}

Predicate<Order> isLargeOrder = order => order.Total >= 5000m;
Console.WriteLine($"Is the event order large: {isLargeOrder(eventOrder)}");
Console.WriteLine($"Extension summary: {eventOrder.GetSummary()}");
Console.WriteLine($"Extension IsLargeOrder(): {eventOrder.IsLargeOrder()}");

PrintHeading("Laboratory Work 6: validation and exception handling");
Console.Write("Enter a positive quantity for a validation demo (press Enter to use 2): ");
var quantityInput = Console.ReadLine();
if (string.IsNullOrWhiteSpace(quantityInput))
{
    quantityInput = "2";
}

if (!int.TryParse(quantityInput, out var validQuantity) || validQuantity <= 0)
{
    Console.WriteLine("Input is not a positive integer. Using the default value 2.");
    validQuantity = 2;
}

var validationOrder = new Order(customers[0]);
var successfulPaymentEvents = 0;
validationOrder.OrderPaid += (_, _) => successfulPaymentEvents++;

validationOrder.AddItem(mouse, validQuantity, logger);
Console.WriteLine($"Correct operation: {validationOrder.Items.Count} item type was added.");

TryOperation("Create product with an empty name", () =>
{
    _ = new Product(" ", 100m, "Accessories");
});

TryOperation("Create product with zero price", () =>
{
    _ = new Product("Invalid product", 0m, "Accessories");
});

var itemCountBeforeInvalidQuantity = validationOrder.Items.Count;
TryOperation("Add item with quantity = -2", () => validationOrder.AddItem(keyboard, -2, logger));
Console.WriteLine($"Order after invalid quantity: status {validationOrder.Status}, item types {validationOrder.Items.Count} (was {itemCountBeforeInvalidQuantity}).");

validationOrder.Pay(new CardPayment("2222"), logger);
Console.WriteLine($"Correct payment: status {validationOrder.Status}, OrderPaid events {successfulPaymentEvents}.");

TryOperation("Change a paid order", () => validationOrder.AddItem(keyboard, 1, logger));
TryOperation("Pay an already paid order", () => validationOrder.Pay(new CardPayment("3333"), logger));

var cancelledOrder = new Order(customers[1]);
cancelledOrder.AddItem(mouse, 1);
cancelledOrder.Cancel();
var failedPaymentEvents = 0;
cancelledOrder.OrderPaid += (_, _) => failedPaymentEvents++;

TryOperation("Pay a cancelled order", () => cancelledOrder.Pay(new CardPayment("4444"), logger));
Console.WriteLine($"Cancelled order after failed payment: status {cancelledOrder.Status}, OrderPaid events {failedPaymentEvents}.");

PrintHeading("Laboratory Work 7: JSON export, import and configuration");
var configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
var configuration = new ConfigurationBuilder()
    .AddJsonFile(configPath, optional: false)
    .Build();

var relativeReportPath = configuration["Storage:FilePath"]
    ?? throw new InvalidOperationException("Storage:FilePath is not configured.");
var writeIndented = bool.TryParse(configuration["Storage:WriteIndented"], out var configuredWriteIndented)
    && configuredWriteIndented;
var reportPath = Path.Combine(AppContext.BaseDirectory, relativeReportPath);

var reports = orders
    .Select(order => new OrderReport(
        order.Id,
        order.Customer.Name,
        order.Total,
        order.GetStatusDescription(),
        order.Items.Count))
    .ToList();

await WriteReportsAsync(reportPath, reports, writeIndented);
var restoredReports = await ReadReportsAsync(reportPath);

Console.WriteLine($"Restored orders: {restoredReports.Count}");
Console.WriteLine($"Restored total: {restoredReports.Sum(report => report.Total):F2} UAH");
foreach (var report in restoredReports.Take(5))
{
    Console.WriteLine($"{report.CustomerName}: {report.Total:F2} UAH, {report.Status}, {report.ItemsCount} item types");
}

var missingReportPath = Path.Combine(Path.GetDirectoryName(reportPath)!, "missing-report.json");
await ReadReportsAsync(missingReportPath);

var invalidReportPath = Path.Combine(Path.GetDirectoryName(reportPath)!, "invalid-report.json");
await File.WriteAllTextAsync(invalidReportPath, "{ invalid JSON }");
await ReadReportsAsync(invalidReportPath);
File.Delete(invalidReportPath);

PrintHeading("Laboratory Work 8: external customer contacts");
var apiBaseUrl = configuration["ExternalApi:BaseUrl"]
    ?? throw new InvalidOperationException("ExternalApi:BaseUrl is not configured.");
var timeoutSeconds = int.TryParse(configuration["ExternalApi:TimeoutSeconds"], out var configuredTimeout)
    ? configuredTimeout
    : 10;

using var httpClient = new HttpClient { BaseAddress = new Uri(apiBaseUrl) };
using var cancellationSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
var stopwatch = Stopwatch.StartNew();

try
{
    Console.WriteLine("Loading external customer contacts...");
    var contactTasks = Enumerable.Range(1, 3)
        .Select(id => GetUserAsync(httpClient, id, cancellationSource.Token));
    var externalUsers = await Task.WhenAll(contactTasks);
    var receivedContacts = externalUsers.OfType<ExternalUser>().ToList();

    Console.WriteLine($"Received: {receivedContacts.Count}");
    foreach (var externalUser in receivedContacts)
    {
        Console.WriteLine($"{externalUser.Id} | {externalUser.Name} | {externalUser.Email}");
    }

    Console.WriteLine("ExternalUser is a reference-contact DTO; it does not create internal Customer records or orders.");
}
catch (OperationCanceledException)
{
    Console.WriteLine($"External API request was cancelled after {timeoutSeconds} seconds.");
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"External API request failed: {ex.Message}");
}
catch (JsonException ex)
{
    Console.WriteLine($"External API returned invalid JSON: {ex.Message}");
}
finally
{
    stopwatch.Stop();
    Console.WriteLine($"External contacts operation took {stopwatch.ElapsedMilliseconds} ms.");
}

await DemonstrateHttpErrorAsync(httpClient);

PrintHeading("1. Orders with total greater than 1000");
foreach (var order in ordersOver1000)
{
    Console.WriteLine($"{order.Customer.Name}: {order.Total:F2} UAH");
}

PrintHeading("2. Top 5 most expensive orders");
foreach (var order in topFiveOrders)
{
    Console.WriteLine($"{order.Customer.Name}: {order.Total:F2} UAH");
}

PrintHeading("3–4. Sales aggregation");
Console.WriteLine($"Total sales: {totalSales:F2} UAH");
Console.WriteLine($"Average order value: {averageOrderTotal:F2} UAH");

PrintHeading("5. Order count by customer");
foreach (var result in orderCountByCustomer)
{
    Console.WriteLine($"{result.Customer}: {result.Count}");
}

PrintHeading("6. Products from all orders");
Console.WriteLine(string.Join(", ", productsFromOrders.Select(product => product.Name)));

PrintHeading("7. Top 3 products by sold quantity");
foreach (var result in topThreeProducts)
{
    Console.WriteLine($"{result.Product}: {result.Quantity} units");
}

PrintHeading("8. Customers who spent more than 5000");
foreach (var result in highSpendingCustomers)
{
    Console.WriteLine($"{result.Customer}: {result.Spending:F2} UAH");
}

PrintHeading("9. Any order greater than 10000");
Console.WriteLine(hasOrderOver10000 ? "Yes" : "No");

PrintHeading("10. Customer spending dictionary");
foreach (var result in spendingByCustomer.OrderByDescending(pair => pair.Value))
{
    var customer = customers.Single(customer => customer.Id == result.Key);
    Console.WriteLine($"{customer.Name} ({result.Key}): {result.Value:F2} UAH");
}

static Customer CreateCustomer(string name, string email, string city) => new()
{
    Name = name,
    Email = email,
    Address = new Address(city, "Central Street", "10", "01001"),
    PhoneNumber = null
};

static Order CreatePaidOrder(
    Customer customer,
    IPaymentMethod paymentMethod,
    params (Product Product, int Quantity)[] items)
{
    var order = new Order(customer);

    foreach (var (product, quantity) in items)
    {
        order.AddItem(product, quantity);
    }

    order.Pay(paymentMethod);
    return order;
}

static void PrintHeading(string heading)
{
    Console.WriteLine();
    Console.WriteLine(heading);
}

static void TryOperation(string operation, Action action)
{
    Console.WriteLine($"Operation: {operation}");

    try
    {
        action();
        Console.WriteLine("Result: completed successfully.");
    }
    catch (ArgumentOutOfRangeException ex)
    {
        Console.WriteLine($"Error (out of range): {ex.Message}");
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"Error (invalid argument): {ex.Message}");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"Error (invalid operation): {ex.Message}");
    }
    finally
    {
        Console.WriteLine("Operation handling finished.");
    }
}

static async Task WriteReportsAsync(string filePath, IReadOnlyCollection<OrderReport> reports, bool writeIndented)
{
    try
    {
        var directoryPath = Path.GetDirectoryName(filePath)
            ?? throw new InvalidOperationException("The report directory cannot be determined.");
        Directory.CreateDirectory(directoryPath);

        var json = JsonSerializer.Serialize(reports, new JsonSerializerOptions
        {
            WriteIndented = writeIndented
        });

        await File.WriteAllTextAsync(filePath, json);
        Console.WriteLine($"JSON report created: {filePath}");
    }
    catch (UnauthorizedAccessException ex)
    {
        Console.WriteLine($"Cannot write the report: {ex.Message}");
    }
    catch (IOException ex)
    {
        Console.WriteLine($"File write error: {ex.Message}");
    }
}

static async Task<IReadOnlyList<OrderReport>> ReadReportsAsync(string filePath)
{
    try
    {
        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<OrderReport>>(json)
            ?? throw new JsonException("The JSON report does not contain an order list.");
    }
    catch (FileNotFoundException)
    {
        Console.WriteLine($"Report file was not found: {filePath}");
    }
    catch (UnauthorizedAccessException ex)
    {
        Console.WriteLine($"Cannot read the report: {ex.Message}");
    }
    catch (JsonException ex)
    {
        Console.WriteLine($"Invalid JSON report: {ex.Message}");
    }
    catch (IOException ex)
    {
        Console.WriteLine($"File read error: {ex.Message}");
    }

    return [];
}

static async Task<ExternalUser?> GetUserAsync(HttpClient client, int id, CancellationToken token)
{
    return await client.GetFromJsonAsync<ExternalUser>($"users/{id}", token);
}

static async Task DemonstrateHttpErrorAsync(HttpClient client)
{
    try
    {
        using var response = await client.GetAsync("not-a-real-endpoint");
        response.EnsureSuccessStatusCode();
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine($"Handled HTTP error: {ex.StatusCode?.ToString() ?? "unknown status"}.");
    }
}
