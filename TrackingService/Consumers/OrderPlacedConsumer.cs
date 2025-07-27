using MassTransit;
using SharedMessages.Messages;


namespace TrackingService.Consumers;

public class OrderPlacedConsumer: IConsumer<OrderPlaced>
{
    public Task Consume(ConsumeContext<OrderPlaced> context)
    {
        Console.WriteLine($"TrackingService Order tracking {context.Message.OrderId}-{context.Message.Quantity}");
        return Task.CompletedTask;
    }
}