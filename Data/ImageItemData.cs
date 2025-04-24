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
    /// Repository encargado de la gestión de la entidad ImageItem en la base de datos.
    /// </summary>
    public class ImageItemData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ImageItemData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de logs.</param>
        public ImageItemData(ApplicationDbContext context, ILogger<ImageItemData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los ImageItems almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de ImageItems.</returns>
        public async Task<IEnumerable<ImageItem>> GetAllAsync()
        {
            return await _context.Set<ImageItem>().ToListAsync();
        }

        /// <summary>
        /// Obtiene un ImageItem por su ID.
        /// </summary>
        /// <param name="id">Identificador único del ImageItem.</param>
        /// <returns>El ImageItem con el ID especificado.</returns>
        public async Task<ImageItem?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<ImageItem>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener ImageItem con ID {id}");
                throw; // Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        /// <summary>
        /// Crea un nuevo ImageItem en la base de datos.
        /// </summary>
        /// <param name="imageItem">Instancia del ImageItem a crear.</param>
        /// <returns>El ImageItem creado.</returns>
        public async Task<ImageItem> CreateAsync(ImageItem imageItem)
        {
            try
            {
                await _context.Set<ImageItem>().AddAsync(imageItem);
                await _context.SaveChangesAsync();
                return imageItem;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el ImageItem {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un ImageItem existente en la base de datos.
        /// </summary>
        /// <param name="imageItem">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(ImageItem imageItem)
        {
            try
            {
                _context.Set<ImageItem>().Update(imageItem);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el ImageItem {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un ImageItem en la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del ImageItem a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var imageItem = await _context.Set<ImageItem>().FindAsync(id);
                if (imageItem == null)
                    return false;

                _context.Set<ImageItem>().Remove(imageItem);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el ImageItem {ex.Message}");
                return false;
            }
        }
    }
}
