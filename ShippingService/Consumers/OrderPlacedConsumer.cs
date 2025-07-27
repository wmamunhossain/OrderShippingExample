using MassTransit;
using SharedMessages.Messages;


namespace ShippingService.Consumers;

public class OrderPlacedConsumer: IConsumer<OrderPlaced>
{
    public Task Consume(ConsumeContext<OrderPlaced> context)
    {
        Console.WriteLine($"ShippingService Order Received {context.Message.OrderId}-{context.Message.Quantity}");
        return Task.CompletedTask;
    }
}