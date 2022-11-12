namespace InvoicinService.Processor.Model
{
    public class Payment
    {
        public decimal Amount { get; set; }
        public string PaymentType { get; set; } = string.Empty;
    }
}