using Microsoft.EntityFrameworkCore;
using Serilog;
using SalesIntel.API.Infrastructure.Data;
using SalesIntel.API.Infrastructure.Extensions;
using SalesIntel.API.Application.Extensions;
using SalesIntel.API.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/salesintel-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS for separate frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173", // Vite default port
                "http://localhost:3000", // Alternative port
                "https://your-frontend-domain.com" // Production domain
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add PostgreSQL
builder.Services.AddDbContext<SalesIntelDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Application Services
builder.Services.AddApplicationServices();

// Add Infrastructure Services
builder.Services.AddInfrastructureServices();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SalesIntelDbContext>();
    await context.Database.EnsureCreatedAsync();
    await SeedData.SeedAsync(context);
}

app.Run();
