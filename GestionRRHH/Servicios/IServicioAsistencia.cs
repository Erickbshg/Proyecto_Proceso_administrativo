using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GestionRRHH.Models;

namespace GestionRRHH.Servicios
{
    public interface IServicioAsistencia
    {
        Task<List<Asistencia>> ObtenerTodasAsync();
        Task<List<Asistencia>> ObtenerPorEmpleadoAsync(int empleadoId);
        Task<List<Asistencia>> ObtenerPorFechaAsync(DateTime fecha);
        Task<Asistencia> ObtenerPorIdAsync(int id);
        Task<bool> RegistrarEntradaAsync(int empleadoId, string observaciones = null);
        Task<bool> RegistrarSalidaAsync(int asistenciaId);
        Task<bool> CrearAsync(Asistencia asistencia);
        Task<bool> ActualizarAsync(Asistencia asistencia);
        Task<bool> EliminarAsync(int id);
    }
}
