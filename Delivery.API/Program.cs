using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Delivery.API.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar la política de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMVC", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// Configure PostgreSQL DbContext
var connectionString = builder.Configuration.GetConnectionString("DeliveryConnection");
builder.Services.AddDbContext<DeliveryAPIContext>(options =>
    options.UseNpgsql(connectionString)
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 2. Habilitar CORS en el pipeline (¡Debe ir justo antes de Authorization!)
app.UseCors("AllowMVC");

app.UseAuthorization();

app.MapControllers();

app.Run();