using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    /// <summary>
    /// Repository encargado de la gestión de la entidad Rol en la base de datos.
    /// </summary>
    public class RolData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RolData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de logs.</param>
        public RolData(ApplicationDbContext context, ILogger<RolData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los Roles almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de Roles.</returns>
        public async Task<IEnumerable<Rol>> GetAllAsync()
        {
            IEnumerable<Rol> lstRoles = await _context.Set<Rol>().ToListAsync();
            return lstRoles;
        }

        public async Task<Rol?> GetByidAsync(int id)
        {
            try
            {
                return await _context.Set<Rol>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener rol con ID{id}");
                throw;// Re-lanza la excepcion para que sea manejada en capas superiores
            }

        }

        /// <summary>
        /// Crea un nuevo Rol en la base de datos.
        /// </summary>
        /// <param name="rol">Instancia del Rol a crear.</param>
        /// <returns>El Rol creado.</returns>
        public async Task<Rol> CreateAsync(Rol rol)
        {
            try
            {
                await _context.Set<Rol>().AddAsync(rol);
                await _context.SaveChangesAsync();
                return rol;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el Rol {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un Rol existente en la base de datos.
        /// </summary>
        /// <param name="rol">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Rol rol)
        {
            try
            {
                _context.Set<Rol>().Update(rol);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el Rol {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un Rol en la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del Rol a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var rol = await _context.Set<Rol>().FindAsync(id);
                if (rol == null)
                    return false;

                _context.Set<Rol>().Remove(rol);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el Rol {ex.Message}");
                return false;
            }
        }
    }
}
