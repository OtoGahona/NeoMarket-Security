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
    /// Repository encargado de la gestión de la entidad MovimientInventory en la base de datos.
    /// </summary>
    public class MovimientInventoryData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MovimientInventoryData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de logs.</param>
        public MovimientInventoryData(ApplicationDbContext context, ILogger<MovimientInventoryData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los MovimientInventories almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de MovimientInventories.</returns>
        public async Task<IEnumerable<MovimientInventory>> GetAllAsync()
        {
            return await _context.Set<MovimientInventory>().ToListAsync();
        }

        /// <summary>
        /// Obtiene un MovimientInventory por su ID.
        /// </summary>
        /// <param name="id">Identificador único del MovimientInventory.</param>
        /// <returns>El MovimientInventory con el ID especificado.</returns>
        public async Task<MovimientInventory?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<MovimientInventory>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener MovimientInventory con ID {id}");
                throw; // Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        /// <summary>
        /// Crea un nuevo MovimientInventory en la base de datos.
        /// </summary>
        /// <param name="movimientInventory">Instancia del MovimientInventory a crear.</param>
        /// <returns>El MovimientInventory creado.</returns>
        public async Task<MovimientInventory> CreateAsync(MovimientInventory movimientInventory)
        {
            try
            {
                await _context.Set<MovimientInventory>().AddAsync(movimientInventory);
                await _context.SaveChangesAsync();
                return movimientInventory;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el MovimientInventory {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un MovimientInventory existente en la base de datos.
        /// </summary>
        /// <param name="movimientInventory">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(MovimientInventory movimientInventory)
        {
            try
            {
                _context.Set<MovimientInventory>().Update(movimientInventory);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el MovimientInventory {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un MovimientInventory en la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del MovimientInventory a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var movimientInventory = await _context.Set<MovimientInventory>().FindAsync(id);
                if (movimientInventory == null)
                    return false;

                _context.Set<MovimientInventory>().Remove(movimientInventory);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el MovimientInventory {ex.Message}");
                return false;
            }
        }
    }
}
