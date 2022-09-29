global using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using BlazorTipz;
using BlazorTipz.Components;
using BlazorTipz.Data;
using BlazorTipz.Models.DbRelay;
using DataLibrary;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages(options => options.RootDirectory = "/Views");
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<IDataAccess, DataAccess>();
builder.Services.AddSingleton<IDbRelay, DbRelay>();
builder.Services.AddSingleton<AuthenticationComponent>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<TokenServerAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<TokenServerAuthenticationStateProvider>());


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

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
