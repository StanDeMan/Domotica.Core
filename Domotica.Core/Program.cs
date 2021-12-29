using Domotica.Core.Config;
using Microsoft.Extensions.Configuration;
using Domotica.Core.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Warning()
    .WriteTo.File(@$"logs\Domotica.Core.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.GetSection(nameof(DeviceConfig)).Bind(new DeviceConfig());

// Add services to the container.
builder.Services.AddMvc(option => option.EnableEndpointRouting = false);
builder.Services.AddSignalR();

using var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseExceptionHandler("/Error");
app.UseFileServer();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<Device>("/Hubs/Device");
});

app.UseMvc();
app.UseMvcWithDefaultRoute();

app.Run();
