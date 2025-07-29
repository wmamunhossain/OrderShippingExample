using MassTransit;
using SharedMessages.Messages;


namespace SagaOrchestration
{
    public class OrderStateMachine: MassTransitStateMachine<OrderState>
    {
        #region states
        public State Submitted { get; set; }
        public State InventoryReserved { get; set; }
        public State PaymentCompleted { get; set; }
        #endregion

        #region events
        public Event<OrderPlaced> OrderPlacedEvent { get; set; }
        public Event<InventoryReserved> InventoryReservedEvent { get; set; }
        public Event<PaymentCompleted> PaymentCompletedEvent { get; set; }
        #endregion


        public OrderStateMachine()
        {
            InstanceState(x => x.CurrentState);
            Event(() => OrderPlacedEvent, x => x.CorrelateById(context => context.Message.OrderId));
            Event(() => InventoryReservedEvent, x => x.CorrelateById(context => context.Message.OrderId));
            Event(() => PaymentCompletedEvent, x => x.CorrelateById(context => context.Message.OrderId));

            Initially(
                When(OrderPlacedEvent)
                    .ThenAsync(async context =>
                    {
                        context.Saga.OrderId = context.Message.OrderId;
                        context.Saga.Quantity = context.Message.Quantity;
                        Console.WriteLine($"Order placed: {context.Message.OrderId}, Quantity: {context.Message.Quantity}");

                    })
                    .TransitionTo(Submitted)
            );

            During(Submitted,
                When(InventoryReservedEvent)
                    .ThenAsync(async context =>
                    {
                        Console.WriteLine($"Inventory reserved for Order: {context.Message.OrderId}");
                    })
                    .TransitionTo(InventoryReserved)
            );

            During(InventoryReserved,
                When(PaymentCompletedEvent)
                    .ThenAsync(async context =>
                    {
                        Console.WriteLine($"Payment completed for Order: {context.Message.OrderId}");
                    })
                    .TransitionTo(PaymentCompleted)
                    .Finalize()
            );

            SetCompletedWhenFinalized();
        }
    }
}
