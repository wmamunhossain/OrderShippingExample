// saga orchestration
using MassTransit;
using SagaOrchestration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .InMemoryRepository();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "learn");
        cfg.ReceiveEndpoint("saga-service", e =>
        {
            e.StateMachineSaga<OrderState>(context);
        });
    });
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.Run();