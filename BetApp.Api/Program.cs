using Microsoft.EntityFrameworkCore;
using BetApp.Api.Data;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    // Serialize/deserialize enums as their string name over HTTP, matching how
    // they are stored in the database (HasConversion<string>()). Without this,
    // System.Text.Json uses numbers and rejects string values like "Home".
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<BetAppContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("BetApp"))
           .UseSnakeCaseNamingConvention());

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
