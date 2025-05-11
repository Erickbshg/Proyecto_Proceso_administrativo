using System.Collections.Generic;
using System.Threading.Tasks;
using GestionRRHH.Models;

namespace GestionRRHH.Servicios
{
    public interface IServicioEmpleado
    {
        Task<List<Empleado>> ObtenerTodosAsync();
        Task<Empleado> ObtenerPorIdAsync(int id);
        Task<bool> CrearAsync(Empleado empleado);
        Task<bool> ActualizarAsync(Empleado empleado);
        Task<bool> EliminarAsync(int id);
    }
}
