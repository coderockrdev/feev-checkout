using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

using FeevCheckout.Data;
using FeevCheckout.Enums;
using FeevCheckout.Libraries.Http;
using FeevCheckout.Middlewares;
using FeevCheckout.Processors.Payments;
using FeevCheckout.Processors.Webhooks;
using FeevCheckout.Queue;
using FeevCheckout.Services;
using FeevCheckout.Services.Payments;
using FeevCheckout.Services.Webhooks;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
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

        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Unauthorized",
                    Detail = "You are not authorized to access this resource."
                };

                await context.Response.WriteAsJsonAsync(problemDetails);
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status403Forbidden,
                    Title = "Forbidden",
                    Detail = "You do not have permission to access this resource."
                };

                await context.Response.WriteAsJsonAsync(problemDetails);
            }
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

// Exception Handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

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
builder.Services.AddHostedService<FeevBoletoResponseFileWorker>();
builder.Services.AddHostedService<TransactionExpirationWorker>();

var app = builder.Build();

app.UseExceptionHandler();

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

    if (!context.Response.HasStarted)
    {
        if (context.Response.StatusCode == StatusCodes.Status404NotFound)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not Found",
                Detail = "The requested resource does not exist."
            };
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        else if (context.Response.StatusCode == StatusCodes.Status405MethodNotAllowed)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status405MethodNotAllowed,
                Title = "Method Not Allowed",
                Detail = "The HTTP method is not allowed for this endpoint."
            };
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
});

app.MapControllers();

if (args.Length >= 2 && args[0] == "establishment" && args[1] == "add")
{
    var isDevelopment = app.Environment.IsDevelopment();

    var inputMethod = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("How do you want to add the establishment?")
            .AddChoices("Interactive prompts", "JSON input")
    );

    string fullName;
    string shortName;
    string cnpj;
    string domain;

    if (inputMethod == "JSON input")
    {
        var blueprint = new
        {
            fullName = "Company Full Name",
            shortName = "ShortName",
            cnpj = "12345678901234",
            domain = "example.com"
        };

        AnsiConsole.MarkupLine("[blue]JSON Blueprint:[/]");
        AnsiConsole.WriteLine(JsonSerializer.Serialize(blueprint, new JsonSerializerOptions { WriteIndented = true }));
        AnsiConsole.WriteLine();

        while (true)
        {
            var jsonInput = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter the [green]JSON[/]:")
                    .PromptStyle("green")
            );

            try
            {
                var json = JsonDocument.Parse(jsonInput);
                var root = json.RootElement;

                var errors = new List<string>();

                if (!root.TryGetProperty("fullName", out var fullNameElement) ||
                    string.IsNullOrWhiteSpace(fullNameElement.GetString()))
                {
                    errors.Add("fullName: cannot be empty.");
                }

                if (!root.TryGetProperty("shortName", out var shortNameElement) ||
                    string.IsNullOrWhiteSpace(shortNameElement.GetString()))
                {
                    errors.Add("shortName: cannot be empty.");
                }
                else if (shortNameElement.GetString()!.Length > 13)
                {
                    errors.Add("shortName: must be at most 13 characters.");
                }

                if (!root.TryGetProperty("cnpj", out var cnpjElement) ||
                    !Regex.IsMatch(cnpjElement.GetString() ?? "", @"^\d{14}$"))
                {
                    errors.Add("cnpj: must contain exactly 14 numeric digits.");
                }

                if (!root.TryGetProperty("domain", out var domainElement) ||
                    string.IsNullOrWhiteSpace(domainElement.GetString()))
                {
                    errors.Add("domain: cannot be empty.");
                }
                else
                {
                    var domainValue = domainElement.GetString()!;
                    var hostToValidate = domainValue;

                    if (isDevelopment && domainValue.Contains(':'))
                    {
                        var parts = domainValue.Split(':');
                        hostToValidate = parts[0];

                        if (!int.TryParse(parts[1], out var port) || port < 1 || port > 65535)
                            errors.Add("domain: invalid port number.");
                    }

                    var hostType = Uri.CheckHostName(hostToValidate);

                    if (hostType == UriHostNameType.Unknown)
                        errors.Add("domain: invalid domain name.");

                    if (!isDevelopment && domainValue.Contains(':'))
                        errors.Add("domain: ports are not allowed in production domains.");
                }

                if (errors.Count > 0)
                {
                    AnsiConsole.MarkupLine("[red]Validation errors:[/]");
                    foreach (var error in errors)
                        AnsiConsole.MarkupLine($"  [red]• {error}[/]");

                    continue;
                }

                fullName = root.GetProperty("fullName").GetString()!;
                shortName = root.GetProperty("shortName").GetString()!;
                cnpj = root.GetProperty("cnpj").GetString()!;
                domain = root.GetProperty("domain").GetString()!;

                break;
            }
            catch (JsonException)
            {
                AnsiConsole.MarkupLine("[red]Invalid JSON format. Please try again.[/]");
            }
        }
    }
    else
    {
        fullName = AnsiConsole.Prompt(
            new TextPrompt<string>("What is the [green]establishment full name[/]?")
                .PromptStyle("green")
                .Validate(name =>
                    string.IsNullOrWhiteSpace(name)
                        ? ValidationResult.Error("Full name cannot be empty.")
                        : ValidationResult.Success())
        );

        shortName = AnsiConsole.Prompt(
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

        cnpj = AnsiConsole.Prompt(
            new TextPrompt<string>("What is the [green]CNPJ[/] (numbers only)?")
                .PromptStyle("green")
                .Validate(cnpjValue =>
                {
                    if (!Regex.IsMatch(cnpjValue, @"^\d{14}$"))
                        return ValidationResult.Error("CNPJ must contain exactly 14 numeric digits.");

                    return ValidationResult.Success();
                })
        );

        domain = AnsiConsole.Prompt(
            new TextPrompt<string>("What is the [green]Domain[/]?")
                .PromptStyle("green")
                .Validate(domainValue =>
                {
                    if (string.IsNullOrWhiteSpace(domainValue))
                        return ValidationResult.Error("Domain cannot be empty.");

                    var hostToValidate = domainValue;

                    if (isDevelopment && domainValue.Contains(':'))
                    {
                        var parts = domainValue.Split(':');
                        hostToValidate = parts[0];

                        if (!int.TryParse(parts[1], out var port) || port < 1 || port > 65535)
                            return ValidationResult.Error("Invalid port number.");
                    }

                    var hostType = Uri.CheckHostName(hostToValidate);

                    if (hostType == UriHostNameType.Unknown)
                        return ValidationResult.Error("Invalid domain name.");

                    if (!isDevelopment && domainValue.Contains(':'))
                        return ValidationResult.Error("Ports are not allowed in production domains.");

                    return ValidationResult.Success();
                })
        );
    }

    using var scope = app.Services.CreateScope();

    var establishmentService = scope.ServiceProvider.GetRequiredService<IEstablishmentService>();

    var establishment = await establishmentService.CreateEstablishment(
        fullName,
        shortName,
        cnpj,
        domain,
        null,
        null,
        null,
        null,
        null
    );

    AnsiConsole.MarkupLine("[green]✔ Establishment created successfully![/]");

    AnsiConsole.MarkupLine($"[yellow]ID:[/] {establishment.Id}");
    AnsiConsole.MarkupLine($"[yellow]Full name:[/] {establishment.FullName}");
    AnsiConsole.MarkupLine($"[yellow]Short name:[/] {establishment.ShortName}");
    AnsiConsole.MarkupLine($"[yellow]CNPJ:[/] {establishment.CNPJ}");
    AnsiConsole.MarkupLine($"[yellow]Client ID:[/] {establishment.ClientId}");
    AnsiConsole.MarkupLine($"[yellow]Client Secret:[/] {establishment.ClientSecret}");

    return;
}

if (args.Length >= 2 && args[0] == "credential" && args[1] == "add")
{
    var establishmentId = AnsiConsole.Prompt(
        new TextPrompt<string>("What is the [green]Establishment ID[/]?")
            .PromptStyle("green")
            .Validate(id =>
            {
                if (!Guid.TryParse(id, out _))
                    return ValidationResult.Error("Invalid GUID format.");

                return ValidationResult.Success();
            })
    );

    using var scope = app.Services.CreateScope();

    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var credentialService = scope.ServiceProvider.GetRequiredService<ICredentialService>();

    var establishment = await context.Establishments.FindAsync(Guid.Parse(establishmentId));

    if (establishment == null)
    {
        AnsiConsole.MarkupLine("[red]✘ Establishment not found![/]");
        return;
    }

    var allMethods = new Dictionary<string, PaymentMethod>
    {
        { "Boleto", PaymentMethod.FeevBoleto },
        { "Pix", PaymentMethod.FeevPix },
        { "Credit Card", PaymentMethod.BraspagCartao }
    };

    while (true)
    {
        var existingCredentials = await context.Credentials
            .Where(c => c.EstablishmentId == establishment.Id)
            .Select(c => c.Method)
            .ToListAsync();

        var availableMethods = allMethods
            .Where(m => !existingCredentials.Contains(m.Value))
            .ToDictionary(m => m.Key, m => m.Value);

        if (availableMethods.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]All payment methods are already configured for this establishment.[/]");
            break;
        }

        var selectedMethod = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Which [green]payment method[/] do you want to add?")
                .AddChoices(availableMethods.Keys)
        );

        var paymentMethod = availableMethods[selectedMethod];

        if (paymentMethod == PaymentMethod.FeevBoleto)
        {
            AnsiConsole.MarkupLine("[blue]Boleto bank information[/]");

            var bankNumber = AnsiConsole.Prompt(
                new TextPrompt<string>("Bank number?")
                    .Validate(number =>
                    {
                        if (!Regex.IsMatch(number, @"^\d{3}$"))
                            return ValidationResult.Error("Bank number must contain exactly 3 digits.");

                        return ValidationResult.Success();
                    })
            );

            var bankAgency = AnsiConsole.Prompt(
                new TextPrompt<string>("Bank agency?")
                    .Validate(agency =>
                        Regex.IsMatch(agency, @"^\d{4}$")
                            ? ValidationResult.Success()
                            : ValidationResult.Error("Bank agency must contain exactly 4 numeric digits."))
            );

            var bankAccount = AnsiConsole.Prompt(
                new TextPrompt<string>("Bank account?")
                    .Validate(account =>
                        Regex.IsMatch(account, @"^\d{5,12}$")
                            ? ValidationResult.Success()
                            : ValidationResult.Error("Bank account must contain between 5 and 12 numeric digits."))
            );

            var boletoData = AnsiConsole.Prompt(
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

            establishment.BankNumber = bankNumber;
            establishment.BankAgency = bankAgency;
            establishment.BankAccount = bankAccount;
            await context.SaveChangesAsync();

            await credentialService.CreateCredential(establishment.Id, PaymentMethod.FeevBoleto, boletoData);
        }
        else if (paymentMethod == PaymentMethod.FeevPix)
        {
            AnsiConsole.MarkupLine("[blue]Pix information[/]");

            var checkingAccountNumber = AnsiConsole.Prompt(
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

            var pixData = AnsiConsole.Prompt(
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

            establishment.CheckingAccountNumber = checkingAccountNumber;
            await context.SaveChangesAsync();

            await credentialService.CreateCredential(establishment.Id, PaymentMethod.FeevPix, pixData);
        }
        else if (paymentMethod == PaymentMethod.BraspagCartao)
        {
            AnsiConsole.MarkupLine("[blue]Credit card information[/]");

            var creditCardData = AnsiConsole.Prompt(
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

            var creditCardProvider = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select the [green]credit card provider[/]:")
                    .PageSize(8)
                    .AddChoices(creditCardProviders)
            );

            await credentialService.CreateCredential(establishment.Id, PaymentMethod.BraspagCartao, creditCardData, creditCardProvider);
        }

        AnsiConsole.MarkupLine($"[green]✔ Credential for {selectedMethod} added successfully![/]");

        var remainingMethods = allMethods.Count - existingCredentials.Count - 1;

        if (remainingMethods == 0)
        {
            AnsiConsole.MarkupLine("[yellow]All payment methods are now configured.[/]");
            break;
        }

        var addAnother = AnsiConsole.Confirm("Do you want to add another credential?", defaultValue: false);

        if (!addAnother)
            break;
    }

    return;
}

app.Run();
