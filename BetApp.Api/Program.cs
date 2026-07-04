using Microsoft.EntityFrameworkCore;
using BetApp.Api.Data;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<BetAppContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("BetApp")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Generates the raw OpenAPI document at /openapi/v1.json
    app.MapOpenApi();
    // Renders the interactive UI at /scalar, consuming the document above
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
