using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

using FeevCheckout.Data;
using FeevCheckout.Enums;
using FeevCheckout.Libraries.Http;
using FeevCheckout.Processors.Payments;
using FeevCheckout.Processors.Webhooks;
using FeevCheckout.Queue;
using FeevCheckout.Services;
using FeevCheckout.Services.Payments;
using FeevCheckout.Services.Webhooks;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Spectre.Console;

var builder = WebApplication.CreateBuilder(args);

var jwtKey = builder.Configuration["AppSettings:JwtKey"]
             ?? throw new InvalidOperationException("JWT Key not found or not specified.");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    // options.EnableSensitiveDataLogging();

    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions =>
        {
            npgsqlOptions.MapEnum<PaymentMethod>();
            npgsqlOptions.MapEnum<PaymentAttemptStatus>();
        });
});

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, token) =>
    {
        document.Components ??= new OpenApiComponents();

        document.Components.SecuritySchemes.TryAdd("Bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });

        return Task.CompletedTask;
    });

    options.AddOperationTransformer((operation, context, cancellationToken) =>
    {
        var hasAuthorize = context.Description.ActionDescriptor.EndpointMetadata
            .OfType<AuthorizeAttribute>()
            .Any();

        if (hasAuthorize)
        {
            operation.Security ??= [];

            operation.Security.Add(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        }

        return Task.CompletedTask;
    });
});

builder.Services.AddGrpcSwagger();

// Payment Services
builder.Services.AddScoped<IBraspagCartaoService, BraspagCartaoService>();
builder.Services.AddScoped<IFeevBoletoService, FeevBoletoService>();
builder.Services.AddScoped<IFeevPixService, FeevPixService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

// Webhook Services
builder.Services.AddScoped<IFeevBoletoCancellationService, FeevBoletoCancellationService>();
builder.Services.AddScoped<IFeevBoletoResponseFileService, FeevBoletoResponseFileService>();

// Webhook Dispatcher Services
builder.Services.AddScoped<ITransactionWebhookDispatcherService, TransactionWebhookDispatcherService>();

// Common Services
builder.Services.AddScoped<ICardBrandPatternService, CardBrandPatternService>();
builder.Services.AddScoped<ICredentialService, CredentialService>();
builder.Services.AddScoped<IEstablishmentService, EstablishmentService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();

// HTTP Clients
builder.Services.AddScoped<IBraspagClient, BraspagClient>();
builder.Services.AddScoped<IFeevClient, FeevClient>();
builder.Services.AddScoped<IFeevBoletoClient, FeevBoletoClient>();
builder.Services.AddScoped<IFeevPixClient, FeevPixClient>();

// Payment Processors
builder.Services.AddScoped<IPaymentProcessor, BraspagCartaoPaymentProcessor>();
builder.Services.AddScoped<IPaymentProcessor, FeevBoletoPaymentProcessor>();
builder.Services.AddScoped<IPaymentProcessor, FeevPixPaymentProcessor>();
builder.Services.AddScoped<PaymentProcessorFactory>();

// Webhook Processors
builder.Services.AddScoped<IWebhookProcessor, FeevBoletoWebhookProcessor>();
builder.Services.AddScoped<WebhookProcessorFactory>();

// Queue
builder.Services.AddHostedService<FeevBoletoResponseFileWoker>();
builder.Services.AddHostedService<FeevTransactionExpirationWorker>();

var app = builder.Build();

app.UseCors("AllowAll");

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
    var fullName = AnsiConsole.Prompt(
        new TextPrompt<string>("What is the [green]establishment full name[/]?")
            .PromptStyle("green")
            .Validate(name =>
                string.IsNullOrWhiteSpace(name)
                    ? ValidationResult.Error("Full name cannot be empty.")
                    : ValidationResult.Success())
    );

    var shortName = AnsiConsole.Prompt(
        new TextPrompt<string>("What is the [green]short name[/] (max 13 chars)?")
            .PromptStyle("green")
            .Validate(name =>
            {
                if (string.IsNullOrWhiteSpace(name))
                    return ValidationResult.Error("Short name cannot be empty.");

                if (name.Length > 13)
                    return ValidationResult.Error("Short name must be at most 13 characters.");

                return ValidationResult.Success();
            })
    );

    var cnpj = AnsiConsole.Prompt(
        new TextPrompt<string>("What is the [green]CNPJ[/] (numbers only)?")
            .PromptStyle("green")
            .Validate(cnpj =>
            {
                if (!Regex.IsMatch(cnpj, @"^\d{14}$"))
                    return ValidationResult.Error("CNPJ must contain exactly 14 numeric digits.");

                return ValidationResult.Success();
            })
    );

    var paymentMethods = AnsiConsole.Prompt(
        new MultiSelectionPrompt<string>()
            .Title("Which [green]payment methods[/] will this establishment use?")
            .AddChoices("Boleto", "Pix", "Credit Card")
            .Required()
    );

    string? bankNumber = null;
    string? bankAgency = null;
    string? bankAccount = null;
    string? boletoData = null;

    if (paymentMethods.Contains("Boleto"))
    {
        AnsiConsole.MarkupLine("[blue]Boleto bank information[/]");

        bankNumber = AnsiConsole.Prompt(
            new TextPrompt<string>("Bank number?")
                .Validate(number =>
                {
                    if (!Regex.IsMatch(number, @"^\d{3}$"))
                        return ValidationResult.Error("Bank number must contain exactly 3 digits.");

                    return ValidationResult.Success();
                })
        );

        bankAgency = AnsiConsole.Prompt(
            new TextPrompt<string>("Bank agency?")
                .Validate(agency =>
                    Regex.IsMatch(agency, @"^\d{4}$")
                        ? ValidationResult.Success()
                        : ValidationResult.Error("Bank agency must contain exactly 4 numeric digits."))
        );

        bankAccount = AnsiConsole.Prompt(
            new TextPrompt<string>("Bank account?")
                .Validate(account =>
                    Regex.IsMatch(account, @"^\d{5,12}$")
                        ? ValidationResult.Success()
                        : ValidationResult.Error("Bank account must contain between 5 and 12 numeric digits."))
        );

        boletoData = AnsiConsole.Prompt(
            new TextPrompt<string>("Provider configuration (JSON):")
                .PromptStyle("green")
                .Validate(json =>
                {
                    try
                    {
                        JsonDocument.Parse(json);
                        return ValidationResult.Success();
                    }
                    catch
                    {
                        return ValidationResult.Error("Invalid JSON format.");
                    }
                })
        );
    }

    string? checkingAccountNumber = null;
    string? pixData = null;

    if (paymentMethods.Contains("Pix"))
    {
        AnsiConsole.MarkupLine("[blue]Pix information[/]");

        checkingAccountNumber = AnsiConsole.Prompt(
            new TextPrompt<string>("Checking account number (Pix)?")
                .Validate(number =>
                {
                    if (string.IsNullOrWhiteSpace(number))
                        return ValidationResult.Error("Checking account number is required for Pix.");

                    if (number.Length > 2)
                        return ValidationResult.Error("Checking account number must be at most 2 characters.");

                    return ValidationResult.Success();
                })
        );

        pixData = AnsiConsole.Prompt(
            new TextPrompt<string>("Provider configuration (JSON):")
                .PromptStyle("green")
                .Validate(json =>
                {
                    try
                    {
                        JsonDocument.Parse(json);
                        return ValidationResult.Success();
                    }
                    catch
                    {
                        return ValidationResult.Error("Invalid JSON format.");
                    }
                })
        );
    }

    string? creditCardData = null;
    string? creditCardProvider = null;

    if (paymentMethods.Contains("Credit Card"))
    {
        AnsiConsole.MarkupLine("[blue]Credit card information[/]");

        creditCardData = AnsiConsole.Prompt(
            new TextPrompt<string>("Provider configuration (JSON):")
                .PromptStyle("green")
                .Validate(json =>
                {
                    try
                    {
                        JsonDocument.Parse(json);
                        return ValidationResult.Success();
                    }
                    catch
                    {
                        return ValidationResult.Error("Invalid JSON format.");
                    }
                })
        );

        var creditCardProviders = new[]
        {
            "Simulado",
            "Cielo30",
            "Getnet",
            "Rede2",
            "GlobalPayments",
            "Stone",
            "PagarMe",
            "Safra2",
            "PagSeguro",
            "FirstData",
            "Sub1",
            "Banorte",
            "Transbank2",
            "Banese",
            "BrasilCard",
            "CredSystem",
            "Credz",
            "DMCard"
        };

        creditCardProvider = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select the [green]credit card provider[/]:")
                .PageSize(8)
                .AddChoices(creditCardProviders)
        );

        AnsiConsole.MarkupLine(
            $"[green]Selected provider:[/] [yellow]{creditCardProvider}[/]");
    }

    using var scope = app.Services.CreateScope();

    var credentialService = scope.ServiceProvider.GetRequiredService<ICredentialService>();
    var establishmentService = scope.ServiceProvider.GetRequiredService<IEstablishmentService>();

    var establishment = await establishmentService.CreateEstablishment(
        fullName,
        shortName,
        cnpj,
        bankNumber,
        bankAgency,
        bankAccount,
        checkingAccountNumber,
        creditCardProvider
    );

    if (!string.IsNullOrEmpty(boletoData))
        await credentialService.CreateCredential(establishment.Id, PaymentMethod.FeevBoleto, boletoData);

    if (!string.IsNullOrEmpty(pixData))
        await credentialService.CreateCredential(establishment.Id, PaymentMethod.FeevPix, pixData);

    if (!string.IsNullOrEmpty(creditCardData))
        await credentialService.CreateCredential(establishment.Id, PaymentMethod.BraspagCartao, creditCardData,
            creditCardProvider);

    AnsiConsole.MarkupLine("[green]✔ Establishment created successfully![/]");

    AnsiConsole.MarkupLine($"[yellow]Full name:[/] {establishment.FullName}");
    AnsiConsole.MarkupLine($"[yellow]Short name:[/] {establishment.ShortName}");
    AnsiConsole.MarkupLine($"[yellow]CNPJ:[/] {establishment.CNPJ}");
    AnsiConsole.MarkupLine($"[yellow]Client ID:[/] {establishment.ClientId}");
    AnsiConsole.MarkupLine($"[yellow]Client Secret:[/] {establishment.ClientSecret}");

    return;
}

app.Run();
