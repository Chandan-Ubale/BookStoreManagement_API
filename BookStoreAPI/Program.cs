using Books_Core.Interface;
using Books_Core.Models;
using Books_Infrastructure.Data;
using Books_Infrastructure.Repositories;
using Books_Services.Services;
using Microsoft.OpenApi.Models;

using Microsoft.Extensions.Logging;
using System.IO;
using Books_Core.Logger;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// 1. Bind appsettings.json -> BookstoreDatabaseSettings
// --------------------
builder.Services.Configure<BookstoreDatabaseSettings>(
    builder.Configuration.GetSection("BookstoreDatabaseSettings"));

// --------------------
// 2. Register infrastructure and services
// --------------------
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookServices, BookService>();

// --------------------
// 3. Configure Logging
// --------------------
// Define log file path
//var logPath = Path.Combine(AppContext.BaseDirectory, "Logs", "log.txt");

// Ensure the Logs folder exists
var logFolder = Path.GetDirectoryName("Logs/log.txt");
if (!string.IsNullOrEmpty(logFolder) && !Directory.Exists("Logs/log.txt"))
{
    Directory.CreateDirectory(logFolder);
}

// Clear default providers (optional) and add custom file logger
builder.Logging.ClearProviders();
builder.Logging.AddProvider(new SimpleFileLoggerProvider("Logs/log.txt"));

// --------------------
// 4. Add controllers + Swagger
// --------------------
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
// 5. Build the app
// --------------------
var app = builder.Build();

// --------------------
// 6. Configure middleware
// --------------------
// Enable Swagger only in development
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
// 7. Run
// --------------------
app.Run();

// --------------------
// 8. Make Program discoverable for integration tests
// --------------------
public partial class Program { }
