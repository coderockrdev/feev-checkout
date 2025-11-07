using FeevCheckout.Data;
using FeevCheckout.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddOpenApi();
builder.Services.AddGrpcSwagger();

builder.Services.AddScoped<ITransactionService, TransactionService>();

var app = builder.Build();

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

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/openapi/v1.json", "Feev Checkout v1");
    });
}

app.UseHttpsRedirection();

// Force all responses to be as JSON.
app.Use(async (context, next) =>
{
    context.Response.ContentType = "application/json";
    await next();
});

app.MapControllers();

app.Run();
