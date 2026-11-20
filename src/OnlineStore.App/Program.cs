using OnlineStore.Domain;

Console.WriteLine("OnlineStore — Laboratory Work 2");
Console.WriteLine("Student: Anton Shkreba");
Console.WriteLine("Group: 301 TK");
Console.WriteLine("Variant: 1");
Console.WriteLine();

var customer = new Customer("Anton Shkreba", "anton.shkreba@student.example");
var mouse = new Product("Wireless Mouse", 799.00m, "Accessories");
var keyboard = new Product("Mechanical Keyboard", 2199.00m, "Accessories");

var cardOrder = new Order(customer);
cardOrder.AddItem(mouse, 2);
cardOrder.AddItem(keyboard, 1);

IPaymentMethod paymentMethod = new CardPayment("4242");
var cardReceipt = cardOrder.Pay(paymentMethod);

Console.WriteLine($"Order {cardOrder.Id}: {cardOrder.Status}");
Console.WriteLine($"Items: {cardOrder.Items.Count}; total: {cardOrder.Total:F2} UAH");
Console.WriteLine($"Paid using: {cardReceipt.PaymentMethod}");

var bankOrder = new Order(customer);
bankOrder.AddItem(mouse, 1);

paymentMethod = new BankTransferPayment("UA213223130000026007233566001");
var bankReceipt = bankOrder.Pay(paymentMethod);

Console.WriteLine();
Console.WriteLine($"Second order paid using: {bankReceipt.PaymentMethod}");
