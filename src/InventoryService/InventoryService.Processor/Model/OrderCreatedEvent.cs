namespace InventoryService.Processor.Model;

internal class OrderCreatedEvent
{
    public Guid OrderId { get; set; }
    public string Action { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Order Order { get; set; }

}
