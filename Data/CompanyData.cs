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
    /// Repository encargado de la gestión de la entidad Company en la base de datos.
    /// </summary>
    public class CompanyData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CompanyData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de logs.</param>
        public CompanyData(ApplicationDbContext context, ILogger<CompanyData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las Companies almacenadas en la base de datos.
        /// </summary>
        /// <returns>Lista de Companies.</returns>
        public async Task<IEnumerable<Company>> GetAllAsync()
        {
            return await _context.Set<Company>().ToListAsync();
        }

        /// <summary>
        /// Obtiene una Company por su ID.
        /// </summary>
        /// <param name="id">Identificador único de la Company.</param>
        /// <returns>La Company con el ID especificado.</returns>
        public async Task<Company?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Company>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener Company con ID {id}");
                throw; // Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        /// <summary>
        /// Crea una nueva Company en la base de datos.
        /// </summary>
        /// <param name="company">Instancia de la Company a crear.</param>
        /// <returns>La Company creada.</returns>
        public async Task<Company> CreateAsync(Company company)
        {
            try
            {
                await _context.Set<Company>().AddAsync(company);
                await _context.SaveChangesAsync();
                return company;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la Company {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza una Company existente en la base de datos.
        /// </summary>
        /// <param name="company">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Company company)
        {
            try
            {
                _context.Set<Company>().Update(company);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la Company {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina una Company en la base de datos.
        /// </summary>
        /// <param name="id">Identificador único de la Company a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var company = await _context.Set<Company>().FindAsync(id);
                if (company == null)
                    return false;

                _context.Set<Company>().Remove(company);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar la Company {ex.Message}");
                return false;
            }
        }
    }
}
