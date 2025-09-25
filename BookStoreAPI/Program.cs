using Books_Core.Interface;
using Books_Core.Models;
using Books_Infrastructure.Data;
using Books_Infrastructure.Repositories;
using Books_Services.Services;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Logging;
using System.IO;
using Books_Core.Logger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// 1. Bind appsettings.json -> BookstoreDatabaseSettings
// --------------------
builder.Services.Configure<BookstoreDatabaseSettings>(
    builder.Configuration.GetSection("BookstoreDatabaseSettings"));

// --------------------
// 2. Register infrastructure + service layer dependencies
// --------------------
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookServices, BookService>();

// --------------------
// 3. Configure Logging
// --------------------
var logFolderPath = Path.Combine(AppContext.BaseDirectory, "Logs");
if (!Directory.Exists(logFolderPath))
{
    Directory.CreateDirectory(logFolderPath);
}

builder.Logging.ClearProviders();
var logFilePath = Path.Combine(logFolderPath, "log.txt");
builder.Logging.AddProvider(new SimpleFileLoggerProvider(logFilePath));

// --------------------
// 4. Configure Authentication (JWT) & Authorization
// --------------------
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSettings["Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT Key is missing in configuration.");
}
var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };

    // --------------------
    // 4a. Custom 401/403 JSON responses
    // --------------------
    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                Status = 401,
                Message = "Authentication required. Please provide a valid JWT token."
            });
            return context.Response.WriteAsync(result);
        },
        OnForbidden = context =>
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                Status = 403,
                Message = "You do not have permission to perform this action."
            });
            return context.Response.WriteAsync(result);
        }
    };
});

builder.Services.AddAuthorization();

// --------------------
// 5. Add Controllers + Swagger (with Bearer auth support)
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

    // Add Bearer token auth to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
});

// --------------------
// 6. Build the app
// --------------------
var app = builder.Build();

// --------------------
// 7. Configure middleware
// --------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookStoreManagement API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// --------------------
// 7a. Global middleware for consistent response messages
// --------------------
app.Use(async (context, next) =>
{
    var originalBodyStream = context.Response.Body;
    using var responseBody = new MemoryStream();
    context.Response.Body = responseBody;

    try
    {
        await next();

        // Only modify non-success responses (status code >= 400)
        if (context.Response.StatusCode >= 400)
        {
            context.Response.ContentType = "application/json";
            responseBody.Seek(0, SeekOrigin.Begin);
            var existingContent = new StreamReader(responseBody).ReadToEnd();
            var message = string.IsNullOrEmpty(existingContent) ? "An error occurred." : existingContent;

            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                Status = context.Response.StatusCode,
                Message = message
            });

            context.Response.Body = originalBodyStream;
            await context.Response.WriteAsync(result);
        }
        else
        {
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        var result = System.Text.Json.JsonSerializer.Serialize(new
        {
            Status = 500,
            Message = ex.Message
        });
        await context.Response.WriteAsync(result);
    }
});

app.MapControllers();

// --------------------
// 8. Run
// --------------------
app.Run();

// --------------------
// Partial Program class for integration testing
// --------------------
public partial class Program { }
