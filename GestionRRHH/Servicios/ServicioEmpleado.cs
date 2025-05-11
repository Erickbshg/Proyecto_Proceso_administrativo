using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GestionRRHH.Models;

namespace GestionRRHH.Servicios
{
    public class ServicioEmpleado : IServicioEmpleado
    {
        private readonly string _rutaArchivo;

        public ServicioEmpleado()
        {
            _rutaArchivo = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "empleados.json");
        }

        private async Task<List<Empleado>> LeerEmpleadosAsync()
        {
            if (!File.Exists(_rutaArchivo))
            {
                return new List<Empleado>();
            }

            string json = await File.ReadAllTextAsync(_rutaArchivo);
            return JsonSerializer.Deserialize<List<Empleado>>(json) ?? new List<Empleado>();
        }

        private async Task GuardarEmpleadosAsync(List<Empleado> empleados)
        {
            string directorio = Path.GetDirectoryName(_rutaArchivo);
            if (!Directory.Exists(directorio))
            {
                Directory.CreateDirectory(directorio);
            }

            string json = JsonSerializer.Serialize(empleados, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_rutaArchivo, json);
        }

        public async Task<List<Empleado>> ObtenerTodosAsync()
        {
            return await LeerEmpleadosAsync();
        }

        public async Task<Empleado> ObtenerPorIdAsync(int id)
        {
            var empleados = await LeerEmpleadosAsync();
            return empleados.FirstOrDefault(e => e.Id == id);
        }

        public async Task<bool> CrearAsync(Empleado empleado)
        {
            var empleados = await LeerEmpleadosAsync();
            
            if (empleados.Any(e => e.Documento == empleado.Documento))
            {
                return false; // Ya existe un empleado con ese documento
            }

            if (empleados.Any())
            {
                empleado.Id = empleados.Max(e => e.Id) + 1;
            }
            else
            {
                empleado.Id = 1;
            }

            empleados.Add(empleado);
            await GuardarEmpleadosAsync(empleados);
            return true;
        }

        public async Task<bool> ActualizarAsync(Empleado empleado)
        {
            var empleados = await LeerEmpleadosAsync();
            var empleadoExistente = empleados.FirstOrDefault(e => e.Id == empleado.Id);
            
            if (empleadoExistente == null)
            {
                return false;
            }

            // Verificar si el nuevo documento ya existe en otro empleado
            if (empleados.Any(e => e.Id != empleado.Id && e.Documento == empleado.Documento))
            {
                return false;
            }

            // Actualizar todos los campos
            empleadoExistente.Nombre = empleado.Nombre;
            empleadoExistente.Apellido = empleado.Apellido;
            empleadoExistente.Documento = empleado.Documento;
            empleadoExistente.Email = empleado.Email;
            empleadoExistente.Telefono = empleado.Telefono;
            empleadoExistente.Direccion = empleado.Direccion;
            empleadoExistente.FechaContratacion = empleado.FechaContratacion;
            empleadoExistente.Departamento = empleado.Departamento;
            empleadoExistente.Cargo = empleado.Cargo;
            empleadoExistente.Salario = empleado.Salario;
            empleadoExistente.Activo = empleado.Activo;

            await GuardarEmpleadosAsync(empleados);
            return true;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var empleados = await LeerEmpleadosAsync();
            var empleado = empleados.FirstOrDefault(e => e.Id == id);
            
            if (empleado == null)
            {
                return false;
            }

            empleados.Remove(empleado);
            await GuardarEmpleadosAsync(empleados);
            return true;
        }
    }
}
