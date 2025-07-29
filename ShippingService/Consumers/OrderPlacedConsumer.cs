using MassTransit;
using SharedMessages.Messages;


namespace ShippingService.Consumers;

public class OrderPlacedConsumer: IConsumer<OrderPlaced>
{
    public Task Consume(ConsumeContext<OrderPlaced> context)
    {
        if(context.Message.Quantity <= 0)
        {
            throw new Exception("Invalid quantity for order shipping");
        }
        Console.WriteLine($"ShippingService Order Received {context.Message.OrderId}-{context.Message.Quantity}");
        return Task.CompletedTask;
    }
}