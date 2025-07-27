using MassTransit;
using ShippingService.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderPlacedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost");
        cfg.ReceiveEndpoint("order-shipping-queue", e =>
        {
            e.ConfigureConsumer<OrderPlacedConsumer>(context);
            #region direct-exchange
            e.Bind("order-placed-direct-exchange", x =>
            {
                x.RoutingKey = "order.shipping";
                x.ExchangeType = "direct";
            });
            #endregion

            #region fanout-exchange
            e.Bind("order-placed-fanout-exchange", x =>
            {
                x.ExchangeType = "fanout";
            });
            #endregion

            #region topic-exchange
            e.Bind("order-placed-topic-exchange", x =>
            {
                x.RoutingKey = "order.#";
                x.ExchangeType = "topic";
            });
            #endregion
        });
    });
});


builder.Services.AddOpenApi();

var app = builder.Build();

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

