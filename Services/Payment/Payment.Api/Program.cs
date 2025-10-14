using FluentValidation;
using FluentValidation.AspNetCore;
using Payment.Api.Contracts;
using Payment.Api.Middlewares;
using Payment.Application;
using Payment.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddApplicationServices();

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<GetTokenRequestValidator>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();