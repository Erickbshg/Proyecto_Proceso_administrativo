using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GestionRRHH.Models;
using GestionRRHH.Servicios;

namespace GestionRRHH.Controllers
{
    public class EmpleadosController : Controller
    {
        private readonly IServicioEmpleado _servicioEmpleado;

        public EmpleadosController(IServicioEmpleado servicioEmpleado)
        {
            _servicioEmpleado = servicioEmpleado;
        }

        // Verificar sesión activa
        private bool VerificarSesion()
        {
            return HttpContext.Session.GetString("UsuarioId") != null;
        }

        public async Task<IActionResult> Index()
        {
            if (!VerificarSesion())
            {
                return RedirectToAction("Index", "Login");
            }

            var empleados = await _servicioEmpleado.ObtenerTodosAsync();
            return View(empleados);
        }

        public IActionResult Crear()
        {
            if (!VerificarSesion())
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Empleado empleado)
        {
            if (!VerificarSesion())
            {
                return RedirectToAction("Index", "Login");
            }

            if (!ModelState.IsValid)
            {
                return View(empleado);
            }

            empleado.Activo = true;
            var resultado = await _servicioEmpleado.CrearAsync(empleado);
            
            if (!resultado)
            {
                ViewBag.Error = "No se pudo crear el empleado. Verifique que el documento no esté duplicado.";
                return View(empleado);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Editar(int id)
        {
            if (!VerificarSesion())
            {
                return RedirectToAction("Index", "Login");
            }

            var empleado = await _servicioEmpleado.ObtenerPorIdAsync(id);
            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(int id, Empleado empleado)
        {
            if (!VerificarSesion())
            {
                return RedirectToAction("Index", "Login");
            }

            if (id != empleado.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(empleado);
            }

            var resultado = await _servicioEmpleado.ActualizarAsync(empleado);
            
            if (!resultado)
            {
                ViewBag.Error = "No se pudo actualizar el empleado. Verifique que el documento no esté duplicado.";
                return View(empleado);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detalles(int id)
        {
            if (!VerificarSesion())
            {
                return RedirectToAction("Index", "Login");
            }

            var empleado = await _servicioEmpleado.ObtenerPorIdAsync(id);
            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }

        public async Task<IActionResult> Eliminar(int id)
        {
            if (!VerificarSesion())
            {
                return RedirectToAction("Index", "Login");
            }

            var empleado = await _servicioEmpleado.ObtenerPorIdAsync(id);
            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }

        [HttpPost, ActionName("Eliminar")]
        public async Task<IActionResult> ConfirmarEliminar(int id)
        {
            if (!VerificarSesion())
            {
                return RedirectToAction("Index", "Login");
            }

            await _servicioEmpleado.EliminarAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
