using System.IO;
using Domotica.Core.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.WebHost.UseWebRoot(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));

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
