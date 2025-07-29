// OrderService
using MassTransit;
using SharedMessages.Messages;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit((x) =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost");

        #region default

        #endregion

        #region direct-exchange
        //cfg.Message<OrderPlaced>(x => x.SetEntityName("order-placed-direct-exchange"));
        //cfg.Publish<OrderPlaced>(x => x.ExchangeType = "direct");
        #endregion

        #region fanout-exchange
        //fg.Message<OrderPlaced>(x => x.SetEntityName("order-placed-fanout-exchange"));
        //cfg.Publish<OrderPlaced>(x => x.ExchangeType = "fanout");
        #endregion

        #region topic-exchange
        //cfg.Message<OrderPlaced>(x => x.SetEntityName("order-placed-topic-exchange"));
        //cfg.Publish<OrderPlaced>(x => x.ExchangeType = "topic");
        #endregion

        #region headers-exchange
        //cfg.Message<OrderPlaced>(x => x.SetEntityName("order-placed-headers-exchange"));
        //cfg.Publish<OrderPlaced>(x => x.ExchangeType = "headers");
        #endregion
    });
});


builder.Services.AddOpenApi();

var app = builder.Build();

app.MapPost("/orders", async (OrderPlaced order, IBus bus) =>
{
    Console.WriteLine("placing orders");
    var orderPlacedMessage = new OrderPlaced(order.OrderId, order.Quantity);

    #region default
        await bus.Publish(orderPlacedMessage);
    #endregion

    #region fanout-exchange
    //await bus.Publish(orderPlacedMessage);
    #endregion

    #region direct-exchange
    //await bus.Publish(orderPlacedMessage, context =>
    //{
    //    var routingKey = orderPlacedMessage.Quantity < 10 ? "order.shipping" : "order.tracking";
    //    context.SetRoutingKey(routingKey);
    //});
    #endregion

    #region topic-exchange
    //await bus.Publish(orderPlacedMessage, context =>
    //{
    //    var routingKey = orderPlacedMessage.Quantity > 10 ? "order.shipping" : "order.regular.tracking";
    //    context.SetRoutingKey(routingKey);
    //});
    #endregion

    #region headers-exchange
    //await bus.Publish(orderPlacedMessage, async context =>
    //{
    //    var headers = new Dictionary<string, object>
    //    {
    //        { "orderId", orderPlacedMessage.OrderId.ToString() },
    //        { "quantity", orderPlacedMessage.Quantity }
    //    };


    //    if(orderPlacedMessage.Quantity > 10)
    //    {
    //        headers.Add("priority", "high");
    //        headers.Add("department", "shipping");
    //    }
    //    else
    //    {
    //        headers.Add("priority", "low");
    //        headers.Add("department", "tracking");
    //    }

    //    await bus.Publish(orderPlacedMessage, context =>
    //    {
    //        var priority = headers.GetValueOrDefault("priority");
    //        var department = headers.GetValueOrDefault("department");
    //        context.Headers.Set("priority", priority);
    //        context.Headers.Set("department", department);
    //    });
    //});
    #endregion

    return Results.Created($"Order {order.OrderId} placed", orderPlacedMessage);
});


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseHttpsRedirection();
app.Run();

public record OrderRequest(Guid OrderId, int Quantity);