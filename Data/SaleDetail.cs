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
    /// Repository encargado de la gestión de la entidad SeleDetail en la base de datos.
    /// </summary>
    public class SeleDetailData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SeleDetailData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de logs.</param>
        public SeleDetailData(ApplicationDbContext context, ILogger<SeleDetailData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los SeleDetails almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de SeleDetails.</returns>
        public async Task<IEnumerable<SaleDetail>> GetAllAsync()
        {
            return await _context.Set<SaleDetail>().ToListAsync();
        }

        /// <summary>
        /// Obtiene un SeleDetail por su ID.
        /// </summary>
        /// <param name="id">Identificador único del SeleDetail.</param>
        /// <returns>El SeleDetail con el ID especificado.</returns>
        public async Task<SaleDetail?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<SaleDetail>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener SeleDetail con ID {id}");
                throw; // Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        /// <summary>
        /// Crea un nuevo SeleDetail en la base de datos.
        /// </summary>
        /// <param name="seleDetail">Instancia del SeleDetail a crear.</param>
        /// <returns>El SeleDetail creado.</returns>
        public async Task<SaleDetail> CreateAsync(SaleDetail seleDetail)
        {
            try
            {
                await _context.Set<SaleDetail>().AddAsync(seleDetail);
                await _context.SaveChangesAsync();
                return seleDetail;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el SeleDetail {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un SeleDetail existente en la base de datos.
        /// </summary>
        /// <param name="seleDetail">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(SaleDetail seleDetail)
        {
            try
            {
                _context.Set<SaleDetail>().Update(seleDetail);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el SeleDetail {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un SeleDetail en la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del SeleDetail a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var seleDetail = await _context.Set<SaleDetail>().FindAsync(id);
                if (seleDetail == null)
                    return false;

                _context.Set<SaleDetail>().Remove(seleDetail);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el SeleDetail {ex.Message}");
                return false;
            }
        }
    }
}

