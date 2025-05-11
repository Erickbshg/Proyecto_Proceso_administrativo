using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GestionRRHH.Models;
using GestionRRHH.Servicios;

namespace GestionRRHH.Controllers
{
    public class LoginController : Controller
    {
        private readonly IServicioUsuario _servicioUsuario;

        public LoginController(IServicioUsuario servicioUsuario)
        {
            _servicioUsuario = servicioUsuario;
        }

        public IActionResult Index()
        {
            // Si ya hay una sesión activa, redirigir al dashboard
            if (HttpContext.Session.GetString("UsuarioId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string nombreUsuario, string contrasena)
        {
            if (string.IsNullOrEmpty(nombreUsuario) || string.IsNullOrEmpty(contrasena))
            {
                ViewBag.Error = "Por favor, ingrese nombre de usuario y contraseña";
                return View();
            }

            var usuario = await _servicioUsuario.AutenticarAsync(nombreUsuario, contrasena);
            if (usuario == null)
            {
                ViewBag.Error = "Nombre de usuario o contraseña incorrectos";
                return View();
            }

            // Guardar información de sesión
            HttpContext.Session.SetString("UsuarioId", usuario.Id.ToString());
            HttpContext.Session.SetString("NombreUsuario", usuario.NombreUsuario);
            HttpContext.Session.SetString("Rol", usuario.Rol);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            // Limpiar la sesión
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
