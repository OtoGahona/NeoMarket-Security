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
    /// Repository encargado de la gestión de la entidad Form en la base de datos.
    /// </summary>
    public class FormData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FormData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de logs.</param>
        public FormData(ApplicationDbContext context, ILogger<FormData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los Forms almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de Forms.</returns>
        public async Task<IEnumerable<Form>> GetAllAsync()
        {
            return await _context.Set<Form>().ToListAsync();
        }

        /// <summary>
        /// Obtiene un Form por su ID.
        /// </summary>
        /// <param name="id">Identificador único del Form.</param>
        /// <returns>El Form con el ID especificado.</returns>
        public async Task<Form?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Form>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener Form con ID {id}");
                throw; // Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        /// <summary>
        /// Crea un nuevo Form en la base de datos.
        /// </summary>
        /// <param name="form">Instancia del Form a crear.</param>
        /// <returns>El Form creado.</returns>
        public async Task<Form> CreateAsync(Form form)
        {
            try
            {
                await _context.Set<Form>().AddAsync(form);
                await _context.SaveChangesAsync();
                return form;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el Form {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un Form existente en la base de datos.
        /// </summary>
        /// <param name="form">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Form form)
        {
            try
            {
                _context.Set<Form>().Update(form);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el Form {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un Form en la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del Form a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var form = await _context.Set<Form>().FindAsync(id);
                if (form == null)
                    return false;

                _context.Set<Form>().Remove(form);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el Form {ex.Message}");
                return false;
            }
        }
    }
}
