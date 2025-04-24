using Business;
using Entity.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de notificaciones en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationBusiness _notificationBusiness;
        private readonly ILogger<NotificationController> _logger;

        /// <summary>
        /// Constructor del controlador de notificaciones
        /// </summary>
        /// <param name="notificationBusiness">Capa de negocio de notificaciones</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public NotificationController(NotificationBusiness notificationBusiness, ILogger<NotificationController> logger)
        {
            _notificationBusiness = notificationBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las notificaciones del sistema
        /// </summary>
        /// <returns>Lista de notificaciones</returns>
        /// <response code="200">Retorna la lista de notificaciones</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<NotificationDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllNotifications()
        {
            try
            {
                var notifications = await _notificationBusiness.GetAllNotificationsAsync();
                return Ok(notifications);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener notificaciones");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una notificación específica por su ID
        /// </summary>
        /// <param name="id">ID de la notificación</param>
        /// <returns>Notificación solicitada</returns>
        /// <response code="200">Retorna la notificación solicitada</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Notificación no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(NotificationDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetNotificationById(int id)
        {
            try
            {
                var notification = await _notificationBusiness.GetNotificationByIdAsync(id);
                return Ok(notification);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para la notificación con ID: {NotificationId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Notificación no encontrada con ID: {NotificationId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener notificación con ID: {NotificationId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva notificación en el sistema
        /// </summary>
        /// <param name="notificationDto">Datos de la notificación a crear</param>
        /// <returns>Notificación creada</returns>
        /// <response code="201">Retorna la notificación creada</response>
        /// <response code="400">Datos de la notificación no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(NotificationDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateNotification([FromBody] NotificationDto notificationDto)
        {
            try
            {
                var createdNotification = await _notificationBusiness.CreateNotificationAsync(notificationDto);
                return CreatedAtAction(nameof(GetNotificationById), new { id = createdNotification.Id }, createdNotification);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear notificación");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear notificación");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza una notificación existente en el sistema
        /// </summary>
        /// <param name="id">ID de la notificación a actualizar</param>
        /// <param name="notificationDto">Datos actualizados de la notificación</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Notificación actualizada correctamente</response>
        /// <response code="400">Datos de la notificación no válidos</response>
        /// <response code="404">Notificación no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateNotification(int id, [FromBody] NotificationDto notificationDto)
        {
            if (id != notificationDto.Id)
            {
                return BadRequest(new { message = "El ID de la notificación no coincide con el ID proporcionado en el cuerpo de la solicitud." });
            }

            try
            {
                var result = await _notificationBusiness.UpdateNotificationAsync(notificationDto);
                return Ok(new { message = "Notificación actualizada correctamente", success = result });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar la notificación con ID: {NotificationId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Notificación no encontrada con ID: {NotificationId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar la notificación con ID: {NotificationId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza campos específicos de una notificación
        /// </summary>
        /// <param name="id">ID de la notificación a actualizar</param>
        /// <param name="updatedFields">Campos a actualizar</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Notificación actualizada correctamente</response>
        /// <response code="400">Datos no válidos</response>
        /// <response code="404">Notificación no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePartialNotification(int id, [FromBody] NotificationDto updatedFields)
        {
            if (updatedFields == null)
            {
                return BadRequest(new { message = "Los datos proporcionados no pueden ser nulos." });
            }

            try
            {
                var result = await _notificationBusiness.UpdatePartialNotificationAsync(id, updatedFields);
                return Ok(new { message = "Notificación actualizada parcialmente correctamente", success = result });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar parcialmente la notificación con ID: {NotificationId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Notificación no encontrada con ID: {NotificationId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente la notificación con ID: {NotificationId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Realiza una eliminación lógica de una notificación (marca como inactiva)
        /// </summary>
        /// <param name="id">ID de la notificación a eliminar lógicamente</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Notificación marcada como inactiva correctamente</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Notificación no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("soft-delete/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SoftDeleteNotification(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID de la notificación debe ser mayor a 0." });
            }

            try
            {
                var result = await _notificationBusiness.SoftDeleteNotificationAsync(id);
                return Ok(new { message = "Notificación marcada como inactiva correctamente", success = result });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Notificación no encontrada con ID: {NotificationId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al realizar la eliminación lógica de la notificación con ID: {NotificationId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina una notificación del sistema
        /// </summary>
        /// <param name="id">ID de la notificación a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Notificación eliminada correctamente</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Notificación no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID de la notificación debe ser mayor a 0." });
            }

            try
            {
                var result = await _notificationBusiness.DeleteNotificationAsync(id);
                return Ok(new { message = "Notificación eliminada correctamente", success = result });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Notificación no encontrada con ID: {NotificationId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar la notificación con ID: {NotificationId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
