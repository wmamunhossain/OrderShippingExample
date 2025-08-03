// InventoryService
using MassTransit;
using SharedMessages.Messages;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderPlacedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "learn");
        cfg.ReceiveEndpoint("order-placed", e =>
        {
            e.ConfigureConsumer<OrderPlacedConsumer>(context);
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


public class OrderPlacedConsumer : IConsumer<OrderPlaced>
{
    async Task IConsumer<OrderPlaced>.Consume(ConsumeContext<OrderPlaced> context)
    {
        Console.WriteLine($"Inventory reserved for Order {context.Message.OrderId}");
        await context.Publish(new InventoryReserved(context.Message.OrderId));
    }
}