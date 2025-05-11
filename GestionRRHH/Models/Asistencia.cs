using System;

namespace GestionRRHH.Models
{
    public class Asistencia
    {
        public int Id { get; set; }
        public int EmpleadoId { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan HoraEntrada { get; set; }
        public TimeSpan? HoraSalida { get; set; }
        public string Estado { get; set; } // Presente, Ausente, Tardanza, etc.
        public string Observaciones { get; set; }
    }
}
