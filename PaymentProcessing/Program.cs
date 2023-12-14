using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PaymentProcessing;
using PaymentProcessing.Database;


// Any design time code executed just need the DB contexts initialized
if (EF.IsDesignTime)
{
    new HostBuilder().Build().Run();
    var payment_context = new PaymentProcessingContext();
    return;
}

var builder = WebApplication.CreateBuilder(args);

var serverVersion = new MySqlServerVersion("8.2.0");

builder.Services.AddDbContext<PaymentProcessingContext>(
    dbContextOptions => dbContextOptions
        .UseMySql(Config.CONNECTION_STRING, serverVersion, options => options.EnableRetryOnFailure(
            maxRetryCount: 10,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null))
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()
);


builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Include a security scheme to be used with Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter your Bearer token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    // Include a security requirement to enforce that the Bearer token is used
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] { }
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config.JWT_SECRET_KEY))
        };
    });


var paymentTransactionQueue = new RabbitMqAsyncPaymentTransactionQueue();
await paymentTransactionQueue.Connect();

builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddSingleton<IAsyncPaymentTransactionPublisherQueue>(paymentTransactionQueue);
builder.Services.AddSingleton<IAsyncPaymentTransactionConsumerQueue>(paymentTransactionQueue);
builder.Services.AddSingleton<PaymentTransactionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "PaymentProcessing V1");

        options.RoutePrefix = string.Empty;
    });
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHttpsRedirection();

// Force initialization for the processor
var processor = app.Services.GetRequiredService<PaymentTransactionService>();

app.Run();