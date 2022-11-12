namespace OrderingService.Application.Model;

public record CreateOrderDto(
    string OrderDetail,
    string CustomerFirstName,
    string CustomerLastName,
    string CustomerAddress,
    decimal PaymentAmount,
    string PaymentType,
    string Description
    );

