using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GestionRRHH.Models;
using GestionRRHH.Servicios;

namespace GestionRRHH.Controllers
{
    public class AsistenciasController : Controller
    {
        private readonly IServicioAsistencia _servicioAsistencia;
        private readonly IServicioEmpleado _servicioEmpleado;

        public AsistenciasController(IServicioAsistencia servicioAsistencia, IServicioEmpleado servicioEmpleado)
        {
            _servicioAsistencia = servicioAsistencia;
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

            var asistencias = await _servicioAsistencia.ObtenerTodasAsync();
            var empleados = await _servicioEmpleado.ObtenerTodosAsync();

            // Agregar información del empleado a cada asistencia para mostrar en la vista
            foreach (var asistencia in asistencias)
            {
                var empleado = empleados.FirstOrDefault(e => e.Id == asistencia.EmpleadoId);
                ViewData[$"NombreEmpleado_{asistencia.Id}"] = empleado != null 
                    ? $"{empleado.Nombre} {empleado.Apellido}" 
                    : "Empleado no encontrado";
            }

            return View(asistencias);
        }

        public async Task<IActionResult> RegistrarEntrada()
        {
            if (!VerificarSesion())
            {
                return RedirectToAction("Index", "Login");
            }

            var empleados = await _servicioEmpleado.ObtenerTodosAsync();
            ViewBag.Empleados = new SelectList(empleados.Where(e => e.Activo), "Id", "Nombre");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarEntrada(int empleadoId, string observaciones)
        {
            if (!VerificarSesion())
            {
                return RedirectToAction("Index", "Login");
            }

            var resultado = await _servicioAsistencia.RegistrarEntradaAsync(empleadoId, observaciones);
            
            if (!resultado)
            {
                var empleados = await _servicioEmpleado.ObtenerTodosAsync();
                ViewBag.Empleados = new SelectList(empleados.Where(e => e.Activo), "Id", "Nombre");
                ViewBag.Error = "No se pudo registrar la entrada. Verifique que el empleado exista y no haya registrado entrada hoy.";
                return View();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RegistrarSalida(int id)
        {
            if (!VerificarSesion())
            {
                return RedirectToAction("Index", "Login");
            }

            var asistencia = await _servicioAsistencia.ObtenerPorIdAsync(id);
            if (asistencia == null)
            {
                return NotFound();
            }

            var empleado = await _servicioEmpleado.ObtenerPorIdAsync(asistencia.EmpleadoId);
            ViewBag.NombreEmpleado = empleado != null 
                ? $"{empleado.Nombre} {empleado.Apellido}" 
                : "Empleado no encontrado";

            return View(asistencia);
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarSalida(int id, string dummy)
        {
            if (!VerificarSesion())
            {
                return RedirectToAction("Index", "Login");
            }

            var resultado = await _servicioAsistencia.RegistrarSalidaAsync(id);
            
            if (!resultado)
            {
                var asistencia = await _servicioAsistencia.ObtenerPorIdAsync(id);
                var empleado = await _servicioEmpleado.ObtenerPorIdAsync(asistencia.EmpleadoId);
                ViewBag.NombreEmpleado = empleado != null 
                    ? $"{empleado.Nombre} {empleado.Apellido}" 
                    : "Empleado no encontrado";
                ViewBag.Error = "No se pudo registrar la salida.";
                return View(asistencia);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Crear()
        {
            if (!VerificarSesion())
            {
                return RedirectToAction("Index", "Login");
            }

            var empleados = await _servicioEmpleado.ObtenerTodosAsync();
            ViewBag.Empleados = new SelectList(empleados.Where(e => e.Activo), "Id", "Nombre");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Asistencia asistencia)
        {
            if (!VerificarSesion())
            {
                return RedirectToAction("Index", "Login");
            }

            if (!ModelState.IsValid)
            {
                var empleados = await _servicioEmpleado.ObtenerTodosAsync();
                ViewBag.Empleados = new SelectList(empleados.Where(e => e.Activo), "Id", "Nombre");
                return View(asistencia);
            }

            var resultado = await _servicioAsistencia.CrearAsync(asistencia);
            
            if (!resultado)
            {
                var empleados = await _servicioEmpleado.ObtenerTodosAsync();
                ViewBag.Empleados = new SelectList(empleados.Where(e => e.Activo), "Id", "Nombre");
                ViewBag.Error = "No se pudo crear la asistencia. Verifique que el empleado exista y no tenga una asistencia registrada para esa fecha.";
                return View(asistencia);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Editar(int id)
        {
            if (!VerificarSesion())
            {
                return RedirectToAction("Index", "Login");
            }

            var asistencia = await _servicioAsistencia.ObtenerPorIdAsync(id);
            if (asistencia == null)
            {
                return NotFound();
            }

            var empleados = await _servicioEmpleado.ObtenerTodosAsync();
            ViewBag.Empleados = new SelectList(empleados, "Id", "Nombre", asistencia.EmpleadoId);
            return View(asistencia);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(int id, Asistencia asistencia)
        {
            if (!VerificarSesion())
            {
                return RedirectToAction("Index", "Login");
            }

            if (id != asistencia.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var empleados = await _servicioEmpleado.ObtenerTodosAsync();
                ViewBag.Empleados = new SelectList(empleados, "Id", "Nombre", asistencia.EmpleadoId);
                return View(asistencia);
            }

            var resultado = await _servicioAsistencia.ActualizarAsync(asistencia);
            
            if (!resultado)
            {
                var empleados = await _servicioEmpleado.ObtenerTodosAsync();
                ViewBag.Empleados = new SelectList(empleados, "Id", "Nombre", asistencia.EmpleadoId);
                ViewBag.Error = "No se pudo actualizar la asistencia.";
                return View(asistencia);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Eliminar(int id)
        {
            if (!VerificarSesion())
            {
                return RedirectToAction("Index", "Login");
            }

            var asistencia = await _servicioAsistencia.ObtenerPorIdAsync(id);
            if (asistencia == null)
            {
                return NotFound();
            }

            var empleado = await _servicioEmpleado.ObtenerPorIdAsync(asistencia.EmpleadoId);
            ViewBag.NombreEmpleado = empleado != null 
                ? $"{empleado.Nombre} {empleado.Apellido}" 
                : "Empleado no encontrado";

            return View(asistencia);
        }

        [HttpPost, ActionName("Eliminar")]
        public async Task<IActionResult> ConfirmarEliminar(int id)
        {
            if (!VerificarSesion())
            {
                return RedirectToAction("Index", "Login");
            }

            await _servicioAsistencia.EliminarAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
