using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GestionRRHH.Models;

namespace GestionRRHH.Servicios
{
    public class ServicioAsistencia : IServicioAsistencia
    {
        private readonly string _rutaArchivo;
        private readonly IServicioEmpleado _servicioEmpleado;

        public ServicioAsistencia(IServicioEmpleado servicioEmpleado)
        {
            _rutaArchivo = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "asistencias.json");
            _servicioEmpleado = servicioEmpleado;
        }

        private async Task<List<Asistencia>> LeerAsistenciasAsync()
        {
            if (!File.Exists(_rutaArchivo))
            {
                return new List<Asistencia>();
            }

            string json = await File.ReadAllTextAsync(_rutaArchivo);
            return JsonSerializer.Deserialize<List<Asistencia>>(json) ?? new List<Asistencia>();
        }

        private async Task GuardarAsistenciasAsync(List<Asistencia> asistencias)
        {
            string directorio = Path.GetDirectoryName(_rutaArchivo);
            if (!Directory.Exists(directorio))
            {
                Directory.CreateDirectory(directorio);
            }

            string json = JsonSerializer.Serialize(asistencias, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_rutaArchivo, json);
        }

        public async Task<List<Asistencia>> ObtenerTodasAsync()
        {
            return await LeerAsistenciasAsync();
        }

        public async Task<List<Asistencia>> ObtenerPorEmpleadoAsync(int empleadoId)
        {
            var asistencias = await LeerAsistenciasAsync();
            return asistencias.Where(a => a.EmpleadoId == empleadoId).ToList();
        }

        public async Task<List<Asistencia>> ObtenerPorFechaAsync(DateTime fecha)
        {
            var asistencias = await LeerAsistenciasAsync();
            return asistencias.Where(a => a.Fecha.Date == fecha.Date).ToList();
        }

        public async Task<Asistencia> ObtenerPorIdAsync(int id)
        {
            var asistencias = await LeerAsistenciasAsync();
            return asistencias.FirstOrDefault(a => a.Id == id);
        }

        public async Task<bool> RegistrarEntradaAsync(int empleadoId, string observaciones = null)
        {
            // Verificar si el empleado existe
            var empleado = await _servicioEmpleado.ObtenerPorIdAsync(empleadoId);
            if (empleado == null || !empleado.Activo)
            {
                return false;
            }

            var asistencias = await LeerAsistenciasAsync();
            
            // Verificar si ya hay una entrada para hoy
            var hoy = DateTime.Today;
            if (asistencias.Any(a => a.EmpleadoId == empleadoId && a.Fecha.Date == hoy))
            {
                return false; // Ya se registró la entrada hoy
            }

            var nuevaAsistencia = new Asistencia
            {
                Id = asistencias.Any() ? asistencias.Max(a => a.Id) + 1 : 1,
                EmpleadoId = empleadoId,
                Fecha = hoy,
                HoraEntrada = DateTime.Now.TimeOfDay,
                Estado = "Presente",
                Observaciones = observaciones
            };

            asistencias.Add(nuevaAsistencia);
            await GuardarAsistenciasAsync(asistencias);
            return true;
        }

        public async Task<bool> RegistrarSalidaAsync(int asistenciaId)
        {
            var asistencias = await LeerAsistenciasAsync();
            var asistencia = asistencias.FirstOrDefault(a => a.Id == asistenciaId);
            
            if (asistencia == null || asistencia.HoraSalida.HasValue)
            {
                return false; // No existe la asistencia o ya se registró la salida
            }

            asistencia.HoraSalida = DateTime.Now.TimeOfDay;
            await GuardarAsistenciasAsync(asistencias);
            return true;
        }

        public async Task<bool> CrearAsync(Asistencia asistencia)
        {
            // Verificar si el empleado existe
            var empleado = await _servicioEmpleado.ObtenerPorIdAsync(asistencia.EmpleadoId);
            if (empleado == null)
            {
                return false;
            }

            var asistencias = await LeerAsistenciasAsync();
            
            // Verificar si ya hay una asistencia para ese empleado en esa fecha
            if (asistencias.Any(a => a.EmpleadoId == asistencia.EmpleadoId && a.Fecha.Date == asistencia.Fecha.Date))
            {
                return false;
            }

            if (asistencias.Any())
            {
                asistencia.Id = asistencias.Max(a => a.Id) + 1;
            }
            else
            {
                asistencia.Id = 1;
            }

            asistencias.Add(asistencia);
            await GuardarAsistenciasAsync(asistencias);
            return true;
        }

        public async Task<bool> ActualizarAsync(Asistencia asistencia)
        {
            var asistencias = await LeerAsistenciasAsync();
            var asistenciaExistente = asistencias.FirstOrDefault(a => a.Id == asistencia.Id);
            
            if (asistenciaExistente == null)
            {
                return false;
            }

            // Actualizar campos
            asistenciaExistente.EmpleadoId = asistencia.EmpleadoId;
            asistenciaExistente.Fecha = asistencia.Fecha;
            asistenciaExistente.HoraEntrada = asistencia.HoraEntrada;
            asistenciaExistente.HoraSalida = asistencia.HoraSalida;
            asistenciaExistente.Estado = asistencia.Estado;
            asistenciaExistente.Observaciones = asistencia.Observaciones;

            await GuardarAsistenciasAsync(asistencias);
            return true;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var asistencias = await LeerAsistenciasAsync();
            var asistencia = asistencias.FirstOrDefault(a => a.Id == id);
            
            if (asistencia == null)
            {
                return false;
            }

            asistencias.Remove(asistencia);
            await GuardarAsistenciasAsync(asistencias);
            return true;
        }
    }
}
