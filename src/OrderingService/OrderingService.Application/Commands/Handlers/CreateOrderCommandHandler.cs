using FluentResults;
using MediatR;
using OrderingService.Application.Model;
using OrderingService.Domain;

namespace OrderingService.Application.Commands.Handlers;

public record CreateOrderCommand(CreateOrderDto CreateOrderDto) : IRequest<Result>;
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result>
{
    private readonly IOrderRepository _orderRepository;

    public CreateOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = Order.CreateNew();
        order.SetOrderDetail(request.CreateOrderDto.OrderDetail);
        order.SetCustomer(request.CreateOrderDto.CustomerFirstName, request.CreateOrderDto.CustomerLastName, request.CreateOrderDto.CustomerAddress);
        order.SetPayment(request.CreateOrderDto.PaymentAmount, request.CreateOrderDto.PaymentType);
        order.SetDescription(request.CreateOrderDto.Description);
        _orderRepository.Create(order);

        List<IDataObject<Entity>> result;

        try
        {
            result = await _orderRepository.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Error saving order").CausedBy(ex));
        }

        var oResult = result.FirstOrDefault(r => r is IDataObject<Order>);
        if(oResult is not null)
        {
            return Result.Ok(); //TODO create concrete result
        }

        return Result.Fail("Error saving order"); //TODO ceate concrete errors
    }
}
