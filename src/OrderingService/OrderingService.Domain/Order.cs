using Newtonsoft.Json;
using OrderingService.Domain.Events;
using OrderingService.Domain.ValueObjects;

namespace OrderingService.Domain;

public class Order : DomainEntity, IAggregateRoot
{
    [JsonProperty]
    public OrderDetail OrderDetail { get; private set; }

    [JsonProperty]
    public Customer Customer { get; private set; }

    [JsonProperty]
    public Payment Payment { get; private set; }

    [JsonProperty]
    public string Description { get; private set; }

    private Order()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTimeOffset.UtcNow;
    }

    private Order(Guid id)
    {
        Id = id;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public static Order CreateNew()
    {
        var order = new Order { IsNew = true };

        // Raise Event
        order.AddEvent(new OrderCreatedEvent(order.Id, order));
        return order;
    }

    public static Order CreateNew(Guid id)
    {
        var order = new Order(id) { IsNew = true };

        // Raise Event
        order.AddEvent(new OrderCreatedEvent(order.Id, order));
        return order;
    }

    public void SetOrderDetail(string detail)
    {
        if (string.IsNullOrWhiteSpace(detail))
            throw new ArgumentException("Detail is invalid");

        OrderDetail = new OrderDetail(detail);

        if (IsNew)
            return;

        AddEvent(new OrderDetailUpdatedEvent(Id, OrderDetail));
        ModifiedAt = DateTimeOffset.UtcNow;
    }

    public void SetCustomer(string firstName, string lastName, string address)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("FirstName is invalid");
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("LastName is invalid");
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address is invalid");

        Customer = new Customer(firstName, lastName, address);

        if (IsNew)
            return;

        AddEvent(new OrderCustomerUpdatedEvent(Id, Customer));
        ModifiedAt = DateTimeOffset.UtcNow;
    }

    public void SetPayment(decimal amount, string paymentType)
    {
        if(amount < 0)
            throw new ArgumentException("Amount is invalid");

        if (string.IsNullOrWhiteSpace(paymentType))
            throw new ArgumentException("PaymentType is invalid");

        Payment = new Payment(amount, paymentType);
       
        if (IsNew) 
            return;

        AddEvent(new OrderPaymentUpdatedEvent(Id, Payment));
        ModifiedAt = DateTimeOffset.UtcNow;
    }

    public void SetDescription(string description)
    {
        Description = description;

        if (IsNew) return;

        AddEvent(new OrderDescriptionUpdatedEvent(Id, Description));
        ModifiedAt = DateTimeOffset.UtcNow;
    }

    public void SetDeleted()
    {
        if (IsNew) return;

        AddEvent(new OrderDeletedEvent(Id));
        DeletedAt = DateTimeOffset.UtcNow;
        Deleted = true;
    }
}
