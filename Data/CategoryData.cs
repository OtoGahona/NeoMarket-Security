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
    /// Repository encargado de la gestión de la entidad Category en la base de datos.
    /// </summary>
    public class CategoryData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CategoryData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de logs.</param>
        public CategoryData(ApplicationDbContext context, ILogger<CategoryData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las Categories almacenadas en la base de datos.
        /// </summary>
        /// <returns>Lista de Categories.</returns>
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Set<Category>().ToListAsync();
        }

        /// <summary>
        /// Obtiene una Category por su ID.
        /// </summary>
        /// <param name="id">Identificador único de la Category.</param>
        /// <returns>La Category con el ID especificado.</returns>
        public async Task<Category?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Category>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener Category con ID {id}");
                throw; // Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        /// <summary>
        /// Crea una nueva Category en la base de datos.
        /// </summary>
        /// <param name="category">Instancia de la Category a crear.</param>
        /// <returns>La Category creada.</returns>
        public async Task<Category> CreateAsync(Category category)
        {
            try
            {
                await _context.Set<Category>().AddAsync(category);
                await _context.SaveChangesAsync();
                return category;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la Category {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza una Category existente en la base de datos.
        /// </summary>
        /// <param name="category">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Category category)
        {
            try
            {
                _context.Set<Category>().Update(category);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la Category {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina una Category en la base de datos.
        /// </summary>
        /// <param name="id">Identificador único de la Category a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var category = await _context.Set<Category>().FindAsync(id);
                if (category == null)
                    return false;

                _context.Set<Category>().Remove(category);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar la Category {ex.Message}");
                return false;
            }
        }
    }
}
