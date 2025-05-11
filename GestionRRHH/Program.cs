using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GestionRRHH.Servicios;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Registrar servicios personalizados
builder.Services.AddSingleton<IServicioUsuario, ServicioUsuario>();
builder.Services.AddSingleton<IServicioEmpleado, ServicioEmpleado>();
builder.Services.AddSingleton<IServicioAsistencia, ServicioAsistencia>();

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

// Asegurar que existan los archivos JSON necesarios
EnsureJsonFilesExist();

app.Run();

void EnsureJsonFilesExist()
{
    string dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "App_Data");
    if (!Directory.Exists(dataDirectory))
    {
        Directory.CreateDirectory(dataDirectory);
    }

    string usuariosPath = Path.Combine(dataDirectory, "usuarios.json");
    if (!File.Exists(usuariosPath))
    {
        File.WriteAllText(usuariosPath, @"[
            {
                ""Id"": 1,
                ""NombreUsuario"": ""admin"",
                ""Contrasena"": ""admin123"",
                ""Rol"": ""Administrador""
            }
        ]");
    }

    string empleadosPath = Path.Combine(dataDirectory, "empleados.json");
    if (!File.Exists(empleadosPath))
    {
        File.WriteAllText(empleadosPath, "[]");
    }

    string asistenciasPath = Path.Combine(dataDirectory, "asistencias.json");
    if (!File.Exists(asistenciasPath))
    {
        File.WriteAllText(asistenciasPath, "[]");
    }
}
