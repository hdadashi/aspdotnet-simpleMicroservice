using Microsoft.Extensions.Logging;
using Notification.Application.Consumers;

var builder = WebApplication.CreateBuilder(args);

// ✅ Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// ✅ Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Background Worker Registration
builder.Services.AddHostedService<PaymentProcessedConsumer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();