using System;
using System.Collections.Generic;

namespace GestionRRHH.Models
{
    public class Empleado
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Documento { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public DateTime FechaContratacion { get; set; }
        public string Departamento { get; set; }
        public string Cargo { get; set; }
        public decimal Salario { get; set; }
        public bool Activo { get; set; }
    }
}
