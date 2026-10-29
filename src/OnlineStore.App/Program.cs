using OnlineStore.Domain;

Console.WriteLine("OnlineStore — Laboratory Work 3");
Console.WriteLine("Student: Anton Shkreba");
Console.WriteLine("Group: 301 TK");
Console.WriteLine("Variant: 1");
Console.WriteLine();

var address = new Address("Kyiv", "Khreshchatyk Street", "1", "01001");
var customer = new Customer
{
    Name = "Anton Shkreba",
    Email = "anton.shkreba@student.example",
    Address = address,
    PhoneNumber = null
};

var mouse = new Product("Wireless Mouse", 799.00m, "Accessories");
var keyboard = new Product("Mechanical Keyboard", 2199.00m, "Accessories");

var cardOrder = new Order(customer);
cardOrder.AddItem(mouse, 2);
cardOrder.AddItem(keyboard, 1);

IPaymentMethod paymentMethod = new CardPayment("4242");
var cardReceipt = cardOrder.Pay(paymentMethod);

Console.WriteLine($"Order status: {cardOrder.GetStatusDescription()}");
Console.WriteLine($"Category: {cardOrder.GetOrderCategory()}");
Console.WriteLine($"Delivery: {cardOrder.GetDeliveryOption()}");
Console.WriteLine($"Paid using: {cardReceipt.PaymentMethod}");

var bankOrder = new Order(customer);
bankOrder.AddItem(mouse, 1);

paymentMethod = new BankTransferPayment("UA213223130000026007233566001");
var bankReceipt = bankOrder.Pay(paymentMethod);

Console.WriteLine();
Console.WriteLine($"Second order status: {bankOrder.GetStatusDescription()}");
Console.WriteLine($"Second order category: {bankOrder.GetOrderCategory()}");
Console.WriteLine($"Second order paid using: {bankReceipt.PaymentMethod}");
