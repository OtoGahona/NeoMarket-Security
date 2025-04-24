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
    /// Repository encargado de la gestión de la entidad Notification en la base de datos.
    /// </summary>
    public class NotificationData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NotificationData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de logs.</param>
        public NotificationData(ApplicationDbContext context, ILogger<NotificationData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las Notifications almacenadas en la base de datos.
        /// </summary>
        /// <returns>Lista de Notifications.</returns>
        public async Task<IEnumerable<Notification>> GetAllAsync()
        {
            return await _context.Set<Notification>().ToListAsync();
        }

        /// <summary>
        /// Obtiene una Notification por su ID.
        /// </summary>
        /// <param name="id">Identificador único de la Notification.</param>
        /// <returns>La Notification con el ID especificado.</returns>
        public async Task<Notification?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Notification>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener Notification con ID {id}");
                throw; // Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        /// <summary>
        /// Crea una nueva Notification en la base de datos.
        /// </summary>
        /// <param name="notification">Instancia de la Notification a crear.</param>
        /// <returns>La Notification creada.</returns>
        public async Task<Notification> CreateAsync(Notification notification)
        {
            try
            {
                await _context.Set<Notification>().AddAsync(notification);
                await _context.SaveChangesAsync();
                return notification;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la Notification {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza una Notification existente en la base de datos.
        /// </summary>
        /// <param name="notification">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Notification notification)
        {
            try
            {
                _context.Set<Notification>().Update(notification);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la Notification {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina una Notification en la base de datos.
        /// </summary>
        /// <param name="id">Identificador único de la Notification a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var notification = await _context.Set<Notification>().FindAsync(id);
                if (notification == null)
                    return false;

                _context.Set<Notification>().Remove(notification);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar la Notification {ex.Message}");
                return false;
            }
        }
    }
}
