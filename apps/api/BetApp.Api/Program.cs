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

// Domain/business logic lives in services, kept out of the controllers.
// Scoped: one instance per HTTP request, matching the DbContext lifetime it uses.
builder.Services.AddScoped<BetApp.Api.Services.CouponService>();

builder.Services.AddDbContext<BetAppContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("BetApp"))
           .UseSnakeCaseNamingConvention()
           // Curated development seed. EF invokes these on Migrate(), EnsureCreated()
           // and on `dotnet ef database update`. The CLI uses the async path; a
           // synchronous Migrate() in code would use the sync path — so both are
           // implemented. SeedData guards against re-seeding a non-empty database.
           .UseSeeding((context, _) =>
               SeedData.Seed((BetAppContext)context))
           .UseAsyncSeeding((context, _, cancellationToken) =>
               SeedData.SeedAsync((BetAppContext)context, cancellationToken)));

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
