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
    /// Repository encargado de la gestión de la entidad Buyout en la base de datos.
    /// </summary>
    public class BuyoutData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BuyoutData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de logs.</param>
        public BuyoutData(ApplicationDbContext context, ILogger<BuyoutData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los Buyouts almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de Buyouts.</returns>
        public async Task<IEnumerable<Buyout>> GetAllAsync()
        {
            return await _context.Set<Buyout>().ToListAsync();
        }

        /// <summary>
        /// Obtiene un Buyout por su ID.
        /// </summary>
        /// <param name="id">Identificador único del Buyout.</param>
        /// <returns>El Buyout con el ID especificado.</returns>
        public async Task<Buyout?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Buyout>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener Buyout con ID {id}");
                throw; // Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        /// <summary>
        /// Crea un nuevo Buyout en la base de datos.
        /// </summary>
        /// <param name="buyout">Instancia del Buyout a crear.</param>
        /// <returns>El Buyout creado.</returns>
        public async Task<Buyout> CreateAsync(Buyout buyout)
        {
            try
            {
                await _context.Set<Buyout>().AddAsync(buyout);
                await _context.SaveChangesAsync();
                return buyout;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el Buyout {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un Buyout existente en la base de datos.
        /// </summary>
        /// <param name="buyout">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Buyout buyout)
        {
            try
            {
                _context.Set<Buyout>().Update(buyout);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el Buyout {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un Buyout en la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del Buyout a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var buyout = await _context.Set<Buyout>().FindAsync(id);
                if (buyout == null)
                    return false;

                _context.Set<Buyout>().Remove(buyout);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el Buyout {ex.Message}");
                return false;
            }
        }
    }
}
