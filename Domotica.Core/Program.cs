using Domotica.Core.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMvc(option => option.EnableEndpointRouting = false);
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseExceptionHandler("/Error");
app.UseHsts();
app.UseHttpsRedirection();
app.UseFileServer();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<Device>("/Hubs/Device");
    endpoints.MapHub<Hello>("/Hubs/Hello");
});
app.UseMvc();
app.UseMvcWithDefaultRoute();
app.UseHsts();

app.Run();
