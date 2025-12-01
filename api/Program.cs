using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using FeevCheckout.Data;
using FeevCheckout.Enums;
using FeevCheckout.Services;
using FeevCheckout.Services.Payments;
using FeevCheckout.Services.Webhooks;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

static string? GetArgValue(string[] args, string key)
{
    var index = Array.IndexOf(args, key);

    return index >= 0 && index < args.Length - 1 ? args[index + 1] : null;
}

var builder = WebApplication.CreateBuilder(args);

var jwtKey = builder.Configuration["AppSettings:JwtKey"]
             ?? throw new InvalidOperationException("JWT Key not found or not specified.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), options =>
    {
        options.MapEnum<PaymentMethod>();
        options.MapEnum<PaymentAttemptStatus>();
    })
);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower));
    });

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddOpenApi();
builder.Services.AddGrpcSwagger();

builder.Services.AddScoped<ICredentialService, CredentialService>();
builder.Services.AddScoped<IEstablishmentService, EstablishmentService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<PaymentProcessorFactory>();
builder.Services.AddScoped<WebhookProcessorFactory>();

builder.Services.AddScoped<IPaymentProcessor, FeevBoletoPaymentProcessor>();
builder.Services.AddScoped<IPaymentProcessor, FeevPixPaymentProcessor>();

builder.Services.AddScoped<IWebhookProcessor, FeevBoletoWebhookProcessor>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Ensure database exists and it's updated.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(swagger => { swagger.SwaggerEndpoint("/openapi/v1.json", "Feev Checkout v1"); });
}

app.UseHttpsRedirection();

// Force all responses to be as JSON.
app.Use(async (context, next) =>
{
    context.Response.ContentType = "application/json";
    await next();
});

app.MapControllers();

if (args.Length > 0 && args[0] == "establishment" && args[1] == "add")
{
    var name = GetArgValue(args, "--name") ?? throw new Exception("Missing --name");

    using var scope = app.Services.CreateScope();
    var service = scope.ServiceProvider.GetRequiredService<IEstablishmentService>();

    var establishment = await service.CreateEstablishment(name);

    Console.WriteLine("Establishment created sucessfully!");
    Console.WriteLine($"Name: {name}");
    Console.WriteLine($"Client ID: {establishment.ClientId}");
    Console.WriteLine($"Client Secret: {establishment.ClientSecret}");

    return;
}

app.Run();
