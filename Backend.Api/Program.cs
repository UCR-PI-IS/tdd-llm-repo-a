using Microsoft.AspNetCore.Builder;
using UCR.ECCI.PI.ThemePark.Backend.DependencyInjection;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCleanArchitectureServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.MapLearningSpaceEndpoints();
app.MapLearningSpaceComponentsEndpoints();

app.Run();
