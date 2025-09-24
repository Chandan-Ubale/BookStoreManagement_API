using Books_Core.Interface;
using Books_Core.Models;
using Books_Infrastructure.Data;
using Books_Infrastructure.Repositories;
using Books_Services.Services;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Logging;
using System.IO;
using Books_Core.Logger;

/// <summary>
/// Entry point of the BookStoreManagement API application.
/// Configures services, middleware, logging, and API endpoints.
/// </summary>
var builder = WebApplication.CreateBuilder(args);

// --------------------
// 1. Bind appsettings.json -> BookstoreDatabaseSettings
// --------------------
builder.Services.Configure<BookstoreDatabaseSettings>(
    builder.Configuration.GetSection("BookstoreDatabaseSettings"));

/// <summary>
/// Registers the infrastructure and service layer dependencies 
/// with the dependency injection (DI) container.
/// </summary>
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookServices, BookService>();

// --------------------
// 2. Configure Logging
// --------------------
/// <summary>
/// Configures logging by clearing default providers and 
/// registering a custom <see cref="SimpleFileLoggerProvider"/>.
/// </summary>
// Define log file path
// var logPath = Path.Combine(AppContext.BaseDirectory, "Logs", "log.txt");

// Ensure the Logs folder exists
// Ensure the Logs folder exists
var logFolderPath = Path.Combine(AppContext.BaseDirectory, "Logs");
if (!Directory.Exists(logFolderPath))
{
    Directory.CreateDirectory(logFolderPath);
}

// Clear default providers (optional) and add custom file logger
builder.Logging.ClearProviders();
var logFilePath = Path.Combine(logFolderPath, "log.txt");
builder.Logging.AddProvider(new SimpleFileLoggerProvider(logFilePath));

// --------------------
// 3. Add controllers + Swagger
// --------------------
/// <summary>
/// Configures MVC controllers and Swagger/OpenAPI documentation.
/// </summary>
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BookStoreManagement API",
        Version = "v1",
        Description = "API for managing books in a bookstore",
        Contact = new OpenApiContact
        {
            Name = "Chandan.G",
            Email = "chandan.g@cmartsolutions.com"
        }
    });
    c.EnableAnnotations();
});

// --------------------
// 4. Build the app
// --------------------
var app = builder.Build();

// --------------------
// 5. Configure middleware
// --------------------
/// <summary>
/// Configures request pipeline, enabling Swagger UI in development environment
/// and adding controller routes with authorization middleware.
/// </summary>
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookStoreManagement API v1");
    });
}

app.UseAuthorization();
app.MapControllers();

// --------------------
// 6. Run
// --------------------
/// <summary>
/// Starts the application.
/// </summary>
app.Run();

/// <summary>
/// Partial Program class is declared to make the entry point discoverable 
/// for integration testing purposes.
/// </summary>
public partial class Program { }
