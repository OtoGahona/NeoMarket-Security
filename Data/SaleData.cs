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
    /// Repository encargado de la gestión de la entidad Sele en la base de datos.
    /// </summary>
    public class SaleData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SaleData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de logs.</param>
        public SaleData(ApplicationDbContext context, ILogger<SaleData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las Seles almacenadas en la base de datos.
        /// </summary>
        /// <returns>Lista de Seles.</returns>
        public async Task<IEnumerable<Sale>> GetAllAsync()
        {
            return await _context.Set<Sale>().ToListAsync();
        }

        /// <summary>
        /// Obtiene una Sele por su ID.
        /// </summary>
        /// <param name="id">Identificador único de la Sele.</param>
        /// <returns>La Sele con el ID especificado.</returns>
        public async Task<Sale?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Sale>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener Sele con ID {id}");
                throw; // Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        /// <summary>
        /// Crea una nueva Sele en la base de datos.
        /// </summary>
        /// <param name="sele">Instancia de la Sele a crear.</param>
        /// <returns>La Sele creada.</returns>
        public async Task<Sale> CreateAsync(Sale sele)
        {
            try
            {
                await _context.Set<Sale>().AddAsync(sele);
                await _context.SaveChangesAsync();
                return sele;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la Sele {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza una Sele existente en la base de datos.
        /// </summary>
        /// <param name="sele">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Sale sele)
        {
            try
            {
                _context.Set<Sale>().Update(sele);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la Sele {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina una Sele en la base de datos.
        /// </summary>
        /// <param name="id">Identificador único de la Sele a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var sele = await _context.Set<Sale>().FindAsync(id);
                if (sele == null)
                    return false;

                _context.Set<Sale>().Remove(sele);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar la Sele {ex.Message}");
                return false;
            }
        }
    }
}

