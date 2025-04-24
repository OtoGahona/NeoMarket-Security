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
    /// Repository encargado de la gestión de la entidad Inventory en la base de datos.
    /// </summary>
    public class InventoryData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InventoryData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de logs.</param>
        public InventoryData(ApplicationDbContext context, ILogger<InventoryData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los Inventories almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de Inventories.</returns>
        public async Task<IEnumerable<Inventory>> GetAllAsync()
        {
            return await _context.Set<Inventory>().ToListAsync();
        }

        /// <summary>
        /// Obtiene un Inventory por su ID.
        /// </summary>
        /// <param name="id">Identificador único del Inventory.</param>
        /// <returns>El Inventory con el ID especificado.</returns>
        public async Task<Inventory?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Inventory>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener Inventory con ID {id}");
                throw; // Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        /// <summary>
        /// Crea un nuevo Inventory en la base de datos.
        /// </summary>
        /// <param name="inventory">Instancia del Inventory a crear.</param>
        /// <returns>El Inventory creado.</returns>
        public async Task<Inventory> CreateAsync(Inventory inventory)
        {
            try
            {
                await _context.Set<Inventory>().AddAsync(inventory);
                await _context.SaveChangesAsync();
                return inventory;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el Inventory {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un Inventory existente en la base de datos.
        /// </summary>
        /// <param name="inventory">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Inventory inventory)
        {
            try
            {
                _context.Set<Inventory>().Update(inventory);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el Inventory {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un Inventory en la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del Inventory a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var inventory = await _context.Set<Inventory>().FindAsync(id);
                if (inventory == null)
                    return false;

                _context.Set<Inventory>().Remove(inventory);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el Inventory {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(Inventory inventory)
        {
            throw new NotImplementedException();
        }
    }
}
