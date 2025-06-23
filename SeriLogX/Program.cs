using Serilog;
using SeriLogX.Logging;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog and bind to ILogger<T>
var loggerConfig = await SerilogConfigurator.BuildLoggerConfigurationAsync(builder.Configuration);
Log.Logger = loggerConfig.CreateLogger();
builder.Host.UseSerilog();

// Build service provider to access ILogger<Program>
var tempProvider = builder.Services.BuildServiceProvider();
var startupLogger = tempProvider.GetRequiredService<ILogger<Program>>();

// ? Log some test messages on app startup
startupLogger.LogInformation("Application is starting up...");
startupLogger.LogInformation("Environment: {env}", builder.Environment.EnvironmentName);
startupLogger.LogWarning("This is a test warning log from startup.");
startupLogger.LogError("This is a test error log from startup.");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
