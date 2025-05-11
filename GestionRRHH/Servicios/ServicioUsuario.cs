using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GestionRRHH.Models;

namespace GestionRRHH.Servicios
{
    public class ServicioUsuario : IServicioUsuario
    {
        private readonly string _rutaArchivo;

        public ServicioUsuario()
        {
            _rutaArchivo = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "usuarios.json");
        }

        private async Task<List<Usuario>> LeerUsuariosAsync()
        {
            if (!File.Exists(_rutaArchivo))
            {
                return new List<Usuario>();
            }

            string json = await File.ReadAllTextAsync(_rutaArchivo);
            return JsonSerializer.Deserialize<List<Usuario>>(json) ?? new List<Usuario>();
        }

        private async Task GuardarUsuariosAsync(List<Usuario> usuarios)
        {
            string directorio = Path.GetDirectoryName(_rutaArchivo);
            if (!Directory.Exists(directorio))
            {
                Directory.CreateDirectory(directorio);
            }

            string json = JsonSerializer.Serialize(usuarios, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_rutaArchivo, json);
        }

        public async Task<List<Usuario>> ObtenerTodosAsync()
        {
            return await LeerUsuariosAsync();
        }

        public async Task<Usuario> ObtenerPorIdAsync(int id)
        {
            var usuarios = await LeerUsuariosAsync();
            return usuarios.FirstOrDefault(u => u.Id == id);
        }

        public async Task<Usuario> AutenticarAsync(string nombreUsuario, string contrasena)
        {
            var usuarios = await LeerUsuariosAsync();
            return usuarios.FirstOrDefault(u => 
                u.NombreUsuario.Equals(nombreUsuario, StringComparison.OrdinalIgnoreCase) && 
                u.Contrasena == contrasena);
        }

        public async Task<bool> CrearAsync(Usuario usuario)
        {
            var usuarios = await LeerUsuariosAsync();
            
            if (usuarios.Any(u => u.NombreUsuario.Equals(usuario.NombreUsuario, StringComparison.OrdinalIgnoreCase)))
            {
                return false; // Ya existe un usuario con ese nombre
            }

            if (usuarios.Any())
            {
                usuario.Id = usuarios.Max(u => u.Id) + 1;
            }
            else
            {
                usuario.Id = 1;
            }

            usuarios.Add(usuario);
            await GuardarUsuariosAsync(usuarios);
            return true;
        }

        public async Task<bool> ActualizarAsync(Usuario usuario)
        {
            var usuarios = await LeerUsuariosAsync();
            var usuarioExistente = usuarios.FirstOrDefault(u => u.Id == usuario.Id);
            
            if (usuarioExistente == null)
            {
                return false;
            }

            // Verificar si el nuevo nombre de usuario ya existe en otro usuario
            if (usuarios.Any(u => u.Id != usuario.Id && 
                                u.NombreUsuario.Equals(usuario.NombreUsuario, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            usuarioExistente.NombreUsuario = usuario.NombreUsuario;
            usuarioExistente.Contrasena = usuario.Contrasena;
            usuarioExistente.Rol = usuario.Rol;

            await GuardarUsuariosAsync(usuarios);
            return true;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var usuarios = await LeerUsuariosAsync();
            var usuario = usuarios.FirstOrDefault(u => u.Id == id);
            
            if (usuario == null)
            {
                return false;
            }

            usuarios.Remove(usuario);
            await GuardarUsuariosAsync(usuarios);
            return true;
        }
    }
}
