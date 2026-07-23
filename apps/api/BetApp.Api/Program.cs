using Microsoft.EntityFrameworkCore;
using BetApp.Api.Data;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Enums serialize as their string name via a [JsonConverter] attribute on each
// enum type (see the Models). That attribute is honored by both the JSON serializer
// and the OpenAPI schema generator, so the wire format and the published contract
// stay in sync. A global converter here would only cover serialization — leaving the
// schema to report `integer` — so it is deliberately omitted.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// CORS is a *browser* enforcement: the API answers fine, but the browser rejects
// a cross-origin response (front on :3000, API on :5075 = different origins)
// unless it carries Access-Control-* headers. This policy is Development-only —
// in production the front is served behind the same origin/proxy, so a permissive
// policy there would only widen the attack surface for no benefit.
const string DevCorsPolicy = "DevFrontend";
builder.Services.AddCors(options =>
    options.AddPolicy(DevCorsPolicy, policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()));

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

    // Allow the Next.js dev server to consume the API from the browser.
    app.UseCors(DevCorsPolicy);
}
else
{
    // Only redirect to HTTPS outside Development. Under the `https` launch profile
    // this would answer the front's http://:5075 calls with a 307 to https://:7083,
    // and a redirected cross-origin request is a common source of silent CORS
    // failures. In Development the front talks plain HTTP, so we skip it.
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
