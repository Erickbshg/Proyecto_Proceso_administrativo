using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GestionRRHH.Models;
using GestionRRHH.Servicios;

namespace GestionRRHH.Controllers
{
    public class HomeController : Controller
    {
        private readonly IServicioEmpleado _servicioEmpleado;
        private readonly IServicioAsistencia _servicioAsistencia;

        public HomeController(IServicioEmpleado servicioEmpleado, IServicioAsistencia servicioAsistencia)
        {
            _servicioEmpleado = servicioEmpleado;
            _servicioAsistencia = servicioAsistencia;
        }

        public async Task<IActionResult> Index()
        {
            // Verificar si hay una sesión activa
            if (HttpContext.Session.GetString("UsuarioId") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Obtener estadísticas para el dashboard
            ViewBag.TotalEmpleados = (await _servicioEmpleado.ObtenerTodosAsync()).Count;
            ViewBag.EmpleadosActivos = (await _servicioEmpleado.ObtenerTodosAsync()).Count(e => e.Activo);
            
            var asistenciasHoy = await _servicioAsistencia.ObtenerPorFechaAsync(DateTime.Today);
            ViewBag.AsistenciasHoy = asistenciasHoy.Count;
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
