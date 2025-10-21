using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;
using UCR.ECCI.PI.ThemePark.Backend.Application;
using UCR.ECCI.PI.ThemePark.Backend.DependencyInjection;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos.ComponentsManagement;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Endpoints;
using UCR.ECCI.PI.ThemePark.Backend.SchemaFilter;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SchemaFilter<LearningComponentDiscriminatorSchemaFilter>();
    c.UseAllOfForInheritance(); 
    c.SelectSubTypesUsing(baseType =>
    {
        if (baseType == typeof(LearningComponentDto))
            return new[] { typeof(ProjectorDto), typeof(WhiteboardDto) };

        if (baseType == typeof(LearningComponentNoIdDto))
            return new[] { typeof(ProjectorNoIdDto), typeof(WhiteboardNoIdDto) };

        return Enumerable.Empty<Type>();
    });
});

builder.Services.AddCleanArchitectureServices(builder.Configuration);


if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    });
}

var app = builder.Build();

// Global exception handler middleware
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.ContentType = "application/json";

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

        var ex = exceptionHandlerPathFeature?.Error;

        context.Response.StatusCode = ex switch
        {
            BadHttpRequestException => StatusCodes.Status400BadRequest,
            JsonException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        var errorResponse = new
        {
            error = ex is BadHttpRequestException || ex is JsonException
                ? "Malformed JSON or invalid request."
                : "An unexpected error occurred.",
            details = ex?.Message
        };

        await context.Response.WriteAsJsonAsync(errorResponse);
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.MapEndpoints();

app.Run();
