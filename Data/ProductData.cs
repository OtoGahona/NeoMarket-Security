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
    /// Repository encargado de la gestión de la entidad Product en la base de datos.
    /// </summary>
    public class ProductData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de logs.</param>
        public ProductData(ApplicationDbContext context, ILogger<ProductData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los Products almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de Products.</returns>
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Set<Product>().ToListAsync();
        }

        /// <summary>
        /// Obtiene un Product por su ID.
        /// </summary>
        /// <param name="id">Identificador único del Product.</param>
        /// <returns>El Product con el ID especificado.</returns>
        public async Task<Product?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Product>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener Product con ID {id}");
                throw; // Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        /// <summary>
        /// Crea un nuevo Product en la base de datos.
        /// </summary>
        /// <param name="product">Instancia del Product a crear.</param>
        /// <returns>El Product creado.</returns>
        public async Task<Product> CreateAsync(Product product)
        {
            try
            {
                await _context.Set<Product>().AddAsync(product);
                await _context.SaveChangesAsync();
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el Product {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un Product existente en la base de datos.
        /// </summary>
        /// <param name="product">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Product product)
        {
            try
            {
                _context.Set<Product>().Update(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el Product {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un Product en la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del Product a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var product = await _context.Set<Product>().FindAsync(id);
                if (product == null)
                    return false;

                _context.Set<Product>().Remove(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el Product {ex.Message}");
                return false;
            }
        }
    }
}
