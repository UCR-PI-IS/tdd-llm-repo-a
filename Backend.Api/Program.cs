using Microsoft.AspNetCore.Builder;
using UCR.ECCI.PI.ThemePark.Backend.DependencyInjection;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

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
/// Maps the Learning Space endpoints to the application.
/// </summary>
app.MapLearningSpaceEndpoints();

app.Run();
