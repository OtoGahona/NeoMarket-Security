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
    /// Repository encargado de la gestión de la entidad Module en la base de datos.
    /// </summary>
    public class ModuleData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ModuleData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de logs.</param>
        public ModuleData(ApplicationDbContext context, ILogger<ModuleData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los Modules almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de Modules.</returns>
        public async Task<IEnumerable<Module>> GetAllAsync()
        {
            return await _context.Set<Module>().ToListAsync();
        }

        /// <summary>
        /// Obtiene un Module por su ID.
        /// </summary>
        /// <param name="id">Identificador único del Module.</param>
        /// <returns>El Module con el ID especificado.</returns>
        public async Task<Module?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Module>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener Module con ID {id}");
                throw; // Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        /// <summary>
        /// Crea un nuevo Module en la base de datos.
        /// </summary>
        /// <param name="module">Instancia del Module a crear.</param>
        /// <returns>El Module creado.</returns>
        public async Task<Module> CreateAsync(Module module)
        {
            try
            {
                await _context.Set<Module>().AddAsync(module);
                await _context.SaveChangesAsync();
                return module;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el Module {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un Module existente en la base de datos.
        /// </summary>
        /// <param name="module">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Module module)
        {
            try
            {
                _context.Set<Module>().Update(module);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el Module {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un Module en la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del Module a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var module = await _context.Set<Module>().FindAsync(id);
                if (module == null)
                    return false;

                _context.Set<Module>().Remove(module);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el Module {ex.Message}");
                return false;
            }
        }
    }
}
