using Microsoft.AspNetCore.Builder;
using UCR.ECCI.IS.ThemePark.Backend.DependencyInjection;
using UCR.ECCI.IS.ThemePark.Backend.Presentation.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

/// <summary>
/// Registers all required services, including Swagger and application layer services.
/// </summary>
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCleanArchitectureServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
/// <summary>
/// Configures the HTTP request pipeline, including Swagger UI for development and 
/// the HTTPS redirection middleware.
/// </summary>
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

/// <summary>
/// Creates a sample forecast for weather data.
/// </summary>
var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
var forecasts = new WeatherForecast[5];

for (int i = 0; i < forecasts.Length; i++)
{
    var date = DateOnly.FromDateTime(DateTime.Now.AddDays(i + 1));
    var temp = Random.Shared.Next(-20, 55);
    var summary = summaries[Random.Shared.Next(summaries.Length)];
    forecasts[i] = new WeatherForecast(date, temp, summary);
}

/// <summary>
/// Maps the weather forecast endpoint to the application, which returns a list of forecasts.
/// </summary>
app.MapGet("/weatherforecast", () => forecasts)
    .WithName("GetWeatherForecast")
    .WithOpenApi();

/// <summary>
/// Maps the Learning Space endpoints to the application.
/// </summary>
app.MapLearningSpaceEndpoints();

app.Run();

/// <summary>
/// Record class representing a weather forecast.
/// </summary>
/// <param name="Date">The date of the forecast.</param>
/// <param name="TemperatureC">The temperature in Celsius.</param>
/// <param name="Summary">A summary of the weather forecast.</param>
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    /// <summary>
    /// Converts the temperature from Celsius to Fahrenheit.
    /// </summary>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
