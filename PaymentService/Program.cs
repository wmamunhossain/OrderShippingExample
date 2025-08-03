// PaymentService
using MassTransit;
using SharedMessages.Messages;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<InventoryReservedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "learn");
        cfg.ReceiveEndpoint("inventory-reserved", e =>
        {
            e.ConfigureConsumer<InventoryReservedConsumer>(context);
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


public class InventoryReservedConsumer : IConsumer<InventoryReserved>
{
    async Task IConsumer<InventoryReserved>.Consume(ConsumeContext<InventoryReserved> context)
    {
        Console.WriteLine($"Payment completed for Order {context.Message.OrderId}");
        await context.Publish(new PaymentCompleted(context.Message.OrderId));
    }
}