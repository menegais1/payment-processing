using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using PaymentProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var builder = WebApplication.CreateBuilder(args);


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

builder.Services.AddScoped<IOrganizationRepository, InMemoryOrganizationRepository>();

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

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Connection settings
var factory = new ConnectionFactory
{
    HostName = "rabbitmq", // Replace with your RabbitMQ server hostname or IP
    Port = 5672,
    UserName = "guest",
    Password = "guest",
};

// Create a connection
// Create a connection
using (var connection = factory.CreateConnection())
{
    // Create a channel
    using (var channel = connection.CreateModel())
    {
        // Declare a topic exchange
        channel.ExchangeDeclare("PaymentTransaction", ExchangeType.Topic);

        // Declare a queue named "PaymentTransactionQueue"
        channel.QueueDeclare("PaymentTransactionQueue", durable: true, exclusive: false, autoDelete: false,
            arguments: null);

        // Bind the queue to the exchange with a routing key "payment.*"
        channel.QueueBind("PaymentTransactionQueue", "PaymentTransaction", "payment.*");

        // Create a consumer to listen for messages
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (sender, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"Received message: {message}");
        };

        // Start listening for messages on the PaymentTransaction topic
        channel.BasicConsume(queue: "PaymentTransactionQueue",
            autoAck: true,
            consumer: consumer);

        // Publish a sample message
        var message = "Payment successful for Order #123";
        var body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange: "PaymentTransaction",
            routingKey: "payment.successful",
            basicProperties: null,
            body: body);

        Console.WriteLine($"Sent message: {message}");
    }
}


app.Run();