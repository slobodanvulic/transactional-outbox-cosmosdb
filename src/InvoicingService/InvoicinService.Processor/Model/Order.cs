namespace InvoicinService.Processor.Model;

internal class Order
{
    public OrderDetail? OrderDetail { get; set; }

    public Customer? Customer { get; set; }

    public Payment? Payment { get; set; }

    public string Description { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"Order detail : {OrderDetail.Detail}, " +
            $"Customer: {Customer.FirstName}, {Customer.LastName}, {Customer.Address}, " +
            $"Payment: {Payment.Amount}, {Payment.PaymentType}," +
            $"Description: {Description}";
    }
}
