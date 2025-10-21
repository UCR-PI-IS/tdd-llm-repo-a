using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.DependencyInjection;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddCleanArchitectureServices(builder.Configuration)
                .AddMudServices();

await builder.Build().RunAsync();
