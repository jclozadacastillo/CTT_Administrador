using CTT_Administrador.Auth;
using CTT_Administrador.Auth.Administrador;
using CTT_Administrador.Auth.Docente;
using CTT_Administrador.Auth.Estudiante;
using CTT_Administrador.Models.ctt;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MySql.Data.MySqlClient;
using Rotativa.AspNetCore;
using System.Data;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
var cn = builder.Configuration.GetConnectionString("ctt");
builder.Services.AddControllersWithViews();
var serverVersion = new MySqlServerVersion(new Version(8, 0,32));
builder.Services.AddDbContext<cttContext>(op => op.UseMySql(cn,serverVersion));
builder.Services.AddScoped<IDbConnection>(op => new MySqlConnection(cn));
ConfigurationHelper.Initialize(builder.Configuration, builder.Environment.ContentRootPath);
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromDays(365);
        options.SlidingExpiration = true;
    });
builder.Services.TryAddScoped<IAuthorizeAdministradorTools, AuthorizeAdministradorTools>();
builder.Services.TryAddScoped<IAuthorizeDocenteTools, AuthorizeDocenteTools>();
builder.Services.TryAddScoped<IAuthorizeEstudianteTools, AuthorizeEstudianteTools>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Administrador}/{action=Index}/{id?}");
RotativaConfiguration.Setup(app.Environment.WebRootPath);
app.Run();