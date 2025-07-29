// tracking service
using MassTransit;
using TrackingService.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMassTransit(x =>
{
    //x.AddConsumer<OrderPlacedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost");
        cfg.ReceiveEndpoint("order-tracking-queue", e =>
        {
            //e.ConfigureConsumer<OrderPlacedConsumer>(context);
            #region direct-exchange
            e.Bind("order-placed-direct-exchange", x =>
            {
                x.RoutingKey = "order.tracking";
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
                x.RoutingKey = "order.*";
                x.ExchangeType = "topic";
            });
            #endregion

            #region topic-exchange
            e.Bind("order-placed-headers-exchange", x =>
            {
                x.SetBindingArgument("department", "tracking");
                x.SetBindingArgument("priority", "low");
                x.SetBindingArgument("x-match", "all");
                x.ExchangeType = "headers";
            });
            #endregion
        });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
