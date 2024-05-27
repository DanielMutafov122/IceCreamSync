using Domain;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Models;
using Service;
using Service.Interfaces;
using Service.Settings;
using Hangfire;
using Domain.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.Configure<ConfigurationOptions>(
    builder.Configuration.GetSection(ConfigurationOptions.Configuration));
builder.Services.AddHttpClient<IUniversalLoaderService, UniversalLoaderService>();
builder.Services.AddScoped<IUniversalLoaderService, UniversalLoaderService>();
builder.Services.AddScoped<IConfigurationSettings, ConfigurationSettings>();
builder.Services.AddScoped<IWorkflowRepository, WorkflowRepository>();
builder.Services.AddMemoryCache();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultValue")));
builder.Services.AddHangfire(options => options.UseDashboardMetrics().
    UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireValue")));
builder.Services.AddHangfireServer();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseHangfireDashboard("/hangfire");
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
