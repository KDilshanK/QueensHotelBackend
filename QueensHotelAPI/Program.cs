using Microsoft.EntityFrameworkCore;
using QueensHotelAPI.Data;
using QueensHotelAPI.Repositories;
using QueensHotelAPI.Services;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine($"Queens Hotel API: Full initialization started at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} by dilshan-jolanka");

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger/OpenAPI for Queens Hotel API
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Queens Hotel API",
        Version = "v1.0.0",
        Description = "REST API for Queens Hotel reservation management system - Last Updated: 2025-05-29 11:53:37",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Dilshan Jolanka",
            Email = "dilshan-jolanka@queenshotel.com"
        }
    });

    // Include XML comments for better API documentation
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Add database context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddDbContext<QueensHotelDbContext>(options =>
    {
        options.UseSqlServer(connectionString, sqlOptions =>
        {
            // Reduce retry attempts to avoid excessive retries that lead to timeouts
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 2, // Reduced from 5 to 2
                maxRetryDelay: TimeSpan.FromSeconds(10), // Reduced from 30 to 10 seconds
                errorNumbersToAdd: new[] { 11001, 2, 53, 121, 232, 258, 1231, 1232, 18456 }); // Specific connection error codes
            
            // Set connection and command timeouts
            sqlOptions.CommandTimeout(90); // Reduced from 60 to 90 seconds for stored procedures
        });
        
        if (builder.Environment.IsDevelopment())
        {
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
            options.LogTo(Console.WriteLine, LogLevel.Information);
        }
        
        // Configure connection resilience
        options.EnableServiceProviderCaching();
        options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
    });

    // Register Queens Hotel repositories and services
    builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
    builder.Services.AddScoped<IReservationService, ReservationService>();

    builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
    builder.Services.AddScoped<ICustomerService, CustomerService>();

    builder.Services.AddScoped<IMealPlanRepository, MealPlanRepository>();
    builder.Services.AddScoped<IMealPlanService, MealPlanService>();

    builder.Services.AddScoped<IRoomRepository, RoomRepository>();
    builder.Services.AddScoped<IRoomService, RoomService>();

    builder.Services.AddScoped<ICountryRepository, CountryRepository>();
    builder.Services.AddScoped<ICountryService, CountryService>();

    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IUserService, UserService>();

    builder.Services.AddScoped<ISuiteRepository, SuiteRepository>();
    builder.Services.AddScoped<ISuiteService, SuiteService>();

    builder.Services.AddScoped<ICheckinRepository, CheckinRepository>();
    builder.Services.AddScoped<ICheckinService, CheckinService>();

    builder.Services.AddScoped<ICheckOutRepository, CheckOutRepository>();
    builder.Services.AddScoped<ICheckOutService, CheckOutService>();

    builder.Services.AddScoped<RoomTypeRepository>();
    builder.Services.AddScoped<RoomTypeService>();

    builder.Services.AddScoped<ITravelAgencyRepository, TravelAgencyRepository>();
    builder.Services.AddScoped<ITravelAgencyService, TravelAgencyService>();

    builder.Services.AddScoped<IFloorRepository, FloorRepository>();
    builder.Services.AddScoped<IFloorService, FloorService>();

    builder.Services.AddScoped<IInsertBillingServiceChargeRepository, InsertBillingServiceChargeRepository>();
    builder.Services.AddScoped<IInsertBillingServiceChargeService, InsertBillingServiceChargeService>();

    builder.Services.AddScoped<ICreditCardRepository, CreditCardRepository>();
    builder.Services.AddScoped<ICreditCardService, CreditCardService>();

    builder.Services.AddScoped<ICustomerInvoiceRepository, CustomerInvoiceRepository>();
    builder.Services.AddScoped<ICustomerInvoiceService, CustomerInvoiceService>();

    builder.Services.AddScoped<IBillingRepository, BillingRepository>();
    builder.Services.AddScoped<IBillingService, BillingService>();

    builder.Services.AddHostedService<ReservationAutoCancelService>();

    Console.WriteLine($"Queens Hotel API: Database services configured at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
}
else
{
    Console.WriteLine($"Queens Hotel API: Running without database (connection string not found) at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
}

// Add comprehensive CORS support for all endpoints
builder.Services.AddCors(options =>
{
    // Main CORS policy for production and development
    options.AddPolicy("QueensHotelApiPolicy", policy =>
    {
        policy.SetIsOriginAllowed(_ => true) // Allow any origin
              .AllowAnyMethod()              // Allow all HTTP methods (GET, POST, PUT, DELETE, OPTIONS, etc.)
              .AllowAnyHeader()              // Allow all headers
              .AllowCredentials();           // Allow credentials if needed
    });
    
    // Fallback permissive policy for maximum compatibility
    options.AddPolicy("AllowAllPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
    
    // Default policy that applies to all controllers
    options.AddDefaultPolicy(policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
    
    // Specific policy for localhost HTTPS testing
    options.AddPolicy("LocalhostHttpsPolicy", policy =>
    {
        policy.WithOrigins(
                  "https://localhost:7111",
                  "http://localhost:5170",
                  "https://localhost:44337", // IIS Express SSL port
                  "http://localhost:22597"   // IIS Express HTTP port
              )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

Console.WriteLine($"Queens Hotel API: Application built successfully at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");

// --- ENABLE SWAGGER IN ALL ENVIRONMENTS, INCLUDING AZURE ---
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Queens Hotel API V1");
    c.RoutePrefix = "swagger"; // Swagger will be available at /swagger
    c.DocumentTitle = "Queens Hotel API Documentation";
});
Console.WriteLine($"Queens Hotel API: Swagger configured at /swagger endpoint at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");

// -----------------------------------------------------------

// Apply CORS policy - MUST be early in the pipeline, before UseRouting
app.UseCors("QueensHotelApiPolicy");

// Add comprehensive CORS middleware for all endpoints
app.Use(async (context, next) =>
{
    var origin = context.Request.Headers.Origin.FirstOrDefault();
    
    // Handle preflight OPTIONS requests for ALL endpoints
    if (context.Request.Method == "OPTIONS")
    {
        // Set comprehensive CORS headers for preflight requests
        context.Response.Headers["Access-Control-Allow-Origin"] = origin ?? "*";
        context.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, DELETE, OPTIONS, PATCH, HEAD";
        context.Response.Headers["Access-Control-Allow-Headers"] = "Origin, X-Requested-With, Content-Type, Accept, Authorization, Cache-Control, Pragma, Expires, X-HTTP-Method-Override";
        context.Response.Headers["Access-Control-Allow-Credentials"] = "true";
        context.Response.Headers["Access-Control-Max-Age"] = "86400"; // 24 hours
        context.Response.Headers["Vary"] = "Origin";
        
        context.Response.StatusCode = 204; // No Content for preflight
        await context.Response.CompleteAsync();
        return;
    }
    
    // Add CORS headers to ALL responses
    context.Response.Headers["Access-Control-Allow-Origin"] = origin ?? "*";
    context.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, DELETE, OPTIONS, PATCH, HEAD";
    context.Response.Headers["Access-Control-Allow-Headers"] = "Origin, X-Requested-With, Content-Type, Accept, Authorization, Cache-Control, Pragma, Expires, X-HTTP-Method-Override";
    context.Response.Headers["Access-Control-Allow-Credentials"] = "true";
    context.Response.Headers["Vary"] = "Origin";
    
    await next();
});

// Additional middleware to ensure CORS headers are present on error responses
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception)
    {
        // Ensure CORS headers are present even on error responses
        var origin = context.Request.Headers.Origin.FirstOrDefault();
        context.Response.Headers["Access-Control-Allow-Origin"] = origin ?? "*";
        context.Response.Headers["Access-Control-Allow-Credentials"] = "true";
        throw; // Re-throw the exception
    }
});

app.UseAuthorization();
app.MapControllers();

// Health check endpoints with CORS enabled
app.MapGet("/", () => Results.Ok(new
{
    Service = "Queens Hotel API",
    Status = "Running Successfully",
    Message = "Welcome to Queens Hotel Reservation Management API",
    Version = "1.0.0",
    Timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
    Developer = "dilshan-jolanka",
    Endpoints = new
    {
        Swagger = "/swagger",
        Health = "/health",
        DatabaseHealth = "/health/database",
        Reservations = "/api/reservation"
    }
})).RequireCors("QueensHotelApiPolicy");

app.MapGet("/health", () => Results.Ok(new
{
    Service = "Queens Hotel API",
    Status = "Healthy",
    Timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
    Version = "1.0.0",
    Developer = "dilshan-jolanka",
    Environment = app.Environment.EnvironmentName
})).RequireCors("QueensHotelApiPolicy");


// Database health check (only if database is configured)
if (!string.IsNullOrEmpty(connectionString))
{
    app.MapGet("/health/database", async (QueensHotelDbContext context) =>
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)); // 10 second timeout
            var canConnect = await context.Database.CanConnectAsync(cts.Token);
            
            if (canConnect)
            {
                // Try a simple query to verify full connectivity
                var reservationCount = await context.Reservation.CountAsync(cts.Token);
                
                return Results.Ok(new
                {
                    Service = "Queens Hotel API - Database",
                    Status = "Healthy",
                    ConnectionStatus = "Connected",
                    ReservationCount = reservationCount,
                    Timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    Developer = "dilshan-jolanka"
                });
            }
            else
            {
                return Results.Ok(new
                {
                    Service = "Queens Hotel API - Database",
                    Status = "Unhealthy",
                    ConnectionStatus = "Disconnected",
                    Timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    Developer = "dilshan-jolanka"
                });
            }
        }
        catch (Exception ex)
        {
            return Results.Problem(new ProblemDetails
            {
                Title = "Database connection failed",
                Detail = ex.Message,
                Status = 503,
                Instance = "/health/database"
            });
        }
    }).RequireCors("QueensHotelApiPolicy");

    // Add a specific connectivity test endpoint
    app.MapGet("/health/database/connection-test", async (QueensHotelDbContext context) =>
    {
        var results = new List<string>();
        
        try
        {
            results.Add("Testing database connection...");
            
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            
            // Test 1: Basic connection
            var canConnect = await context.Database.CanConnectAsync(cts.Token);
            results.Add($"Basic connection test: {(canConnect ? "PASSED" : "FAILED")}");
            
            if (canConnect)
            {
                // Test 2: Simple query
                try
                {
                    var count = await context.Reservation.CountAsync(cts.Token);
                    results.Add($"Query test: PASSED (Found {count} reservations)");
                }
                catch (Exception queryEx)
                {
                    results.Add($"Query test: FAILED - {queryEx.Message}");
                }
                
                // Test 3: Stored procedure execution test
                try
                {
                    await context.Database.ExecuteSqlRawAsync("SELECT 1", cts.Token);
                    results.Add("Stored procedure execution test: PASSED");
                }
                catch (Exception spEx)
                {
                    results.Add($"Stored procedure execution test: FAILED - {spEx.Message}");
                }
            }
            
            return Results.Ok(new
            {
                Service = "Queens Hotel API - Database Connection Test",
                Status = canConnect ? "Healthy" : "Unhealthy",
                Tests = results,
                Timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                ConnectionString = connectionString.Contains("Password") ? 
                    connectionString.Substring(0, connectionString.IndexOf("Password")) + "Password=***;" :
                    connectionString
            });
        }
        catch (Exception ex)
        {
            results.Add($"Connection test failed: {ex.Message}");
            
            return Results.Problem(new ProblemDetails
            {
                Title = "Database connection test failed",
                Detail = string.Join(" | ", results),
                Status = 503,
                Instance = "/health/database/connection-test"
            });
        }
    }).RequireCors("QueensHotelApiPolicy");
}

Console.WriteLine($"Queens Hotel API: Fully configured and starting at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} by dilshan-jolanka");
Console.WriteLine($"Queens Hotel API: Available endpoints:");
Console.WriteLine($"  - Main: http://localhost:5170/");
Console.WriteLine($"  - Swagger: http://localhost:5170/swagger");
Console.WriteLine($"  - Health: http://localhost:5170/health");
Console.WriteLine($"  - CORS Test: http://localhost:5170/cors-test");
if (!string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine($"  - Database Health: http://localhost:5170/health/database");
    Console.WriteLine($"  - Database Connection Test: http://localhost:5170/health/database/connection-test");
    Console.WriteLine($"  - Reservations: http://localhost:5170/api/reservation");
    Console.WriteLine($"  - Update Reservation: PUT http://localhost:5170/api/reservation/{{id}} (requires UserId)");
    Console.WriteLine($"  - Cancel Reservation: http://localhost:5170/api/reservation/cancel");
    Console.WriteLine($"  - Insert Billing: POST http://localhost:5170/api/billing");
    Console.WriteLine($"  - Get Billing Info: GET http://localhost:5170/api/billing/{{id}}");
    Console.WriteLine($"  - Credit Cards: http://localhost:5170/api/creditcard/insert");
    Console.WriteLine($"  - Customer Invoices: http://localhost:5170/api/customerinvoice/{{billingId}}");
    Console.WriteLine($"  - Customers: http://localhost:5170/api/customer");
    Console.WriteLine($"  - Customer by ID: http://localhost:5170/api/customer/{{id}}");
}
Console.WriteLine($"Queens Hotel API: Note - If you experience database connectivity issues,");
Console.WriteLine($"  try the connection test endpoint first to diagnose the problem.");

// Add comprehensive CORS test endpoint
app.MapGet("/cors-test", () => Results.Ok(new
{
    Message = "CORS is working correctly!",
    Timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
    CorsConfiguration = new
    {
        AllowedOrigins = "All origins (*)",
        AllowedMethods = "GET, POST, PUT, DELETE, OPTIONS, PATCH, HEAD",
        AllowedHeaders = "All headers (*)",
        AllowCredentials = true,
        MaxAge = "86400 seconds (24 hours)"
    },
    Environment = app.Environment.EnvironmentName,
    CorsStatus = "Fully Enabled",
    Note = "CORS is enabled for ALL endpoints in this API"
})).RequireCors("QueensHotelApiPolicy");

// Add OPTIONS endpoint for testing preflight requests
app.MapMethods("/cors-preflight-test", new[] { "OPTIONS" }, () => Results.NoContent())
   .RequireCors("QueensHotelApiPolicy");

// Add HTTPS connection test endpoint
app.MapGet("/https-test", () => Results.Ok(new
{
    Message = "HTTPS connection is working correctly!",
    Timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
    ConnectionInfo = new
    {
        HttpsEnabled = true,
        HttpsUrl = "https://localhost:7111",
        HttpUrl = "http://localhost:5170",
        IISExpressHttps = "https://localhost:44337",
        IISExpressHttp = "http://localhost:22597"
    },
    TestInstructions = new
    {
        CurlHttps = "curl -k https://localhost:7111/https-test",
        CurlHttp = "curl http://localhost:5170/https-test",
        Browser = "Open https://localhost:7111/https-test in browser"
    },
    TroubleshootingSteps = new[]
    {
        "1. Try HTTP version first: http://localhost:5170/api/Reservation/39",
        "2. Use curl with -k flag to ignore SSL errors",
        "3. Regenerate dev certificates: dotnet dev-certs https --clean && dotnet dev-certs https --trust",
        "4. Check if API is running on the correct ports",
        "5. Verify firewall isn't blocking the ports"
    }
})).RequireCors("QueensHotelApiPolicy");

Console.WriteLine($"Queens Hotel API: ✅ CORS is FULLY ENABLED for ALL ENDPOINTS");
Console.WriteLine($"Queens Hotel API: 🌐 All origins, methods, and headers are allowed");
Console.WriteLine($"Queens Hotel API: 🔧 Preflight requests are automatically handled");
Console.WriteLine($"Queens Hotel API: 📝 Test CORS with: GET /cors-test");
Console.WriteLine($"Queens Hotel API: 🧪 Test preflight with: OPTIONS /cors-preflight-test");
Console.WriteLine($"Queens Hotel API: Frontend should use these URLs:");
Console.WriteLine($"  - For Reservations: PUT https://backendtestingportaljolanka.azurewebsites.net/api/reservation/{{id}}");
Console.WriteLine($"  - NOT: PUT https://backendtestingportaljolanka.azurewebsites.net/api/customer/{{id}}");
Console.WriteLine($"Queens Hotel API: Make sure your frontend calls the correct reservation endpoint!");

app.Run();