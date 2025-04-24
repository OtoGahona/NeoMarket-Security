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
    /// Controlador para la gestión de movimientos de inventario en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class MovimientInventoryController : ControllerBase
    {
        private readonly MovimientInventoryBusiness _movimientInventoryBusiness;
        private readonly ILogger<MovimientInventoryController> _logger;

        /// <summary>
        /// Constructor del controlador de movimientos de inventario
        /// </summary>
        /// <param name="movimientInventoryBusiness">Capa de negocio de movimientos de inventario</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public MovimientInventoryController(MovimientInventoryBusiness movimientInventoryBusiness, ILogger<MovimientInventoryController> logger)
        {
            _movimientInventoryBusiness = movimientInventoryBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los movimientos de inventario del sistema
        /// </summary>
        /// <returns>Lista de movimientos de inventario</returns>
        /// <response code="200">Retorna la lista de movimientos de inventario</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MovimientInventoryDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllMovimientInventories()
        {
            try
            {
                var movimientInventories = await _movimientInventoryBusiness.GetAllMovimientInventoryAsync();
                return Ok(movimientInventories);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener movimientos de inventario");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un movimiento de inventario específico por su ID
        /// </summary>
        /// <param name="id">ID del movimiento de inventario</param>
        /// <returns>Movimiento de inventario solicitado</returns>
        /// <response code="200">Retorna el movimiento de inventario solicitado</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Movimiento de inventario no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MovimientInventoryDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetMovimientInventoryById(int id)
        {
            try
            {
                var movimientInventory = await _movimientInventoryBusiness.GetMovimientInventoryByIdAsync(id);
                return Ok(movimientInventory);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el movimiento de inventario con ID: {MovimientInventoryId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Movimiento de inventario no encontrado con ID: {MovimientInventoryId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener movimiento de inventario con ID: {MovimientInventoryId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo movimiento de inventario en el sistema
        /// </summary>
        /// <param name="movimientInventoryDto">Datos del movimiento de inventario a crear</param>
        /// <returns>Movimiento de inventario creado</returns>
        /// <response code="201">Retorna el movimiento de inventario creado</response>
        /// <response code="400">Datos del movimiento de inventario no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(MovimientInventoryDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateMovimientInventory([FromBody] MovimientInventoryDto movimientInventoryDto)
        {
            try
            {
                var createdMovimientInventory = await _movimientInventoryBusiness.CreateMovimientInventoryAsync(movimientInventoryDto);
                return CreatedAtAction(nameof(GetMovimientInventoryById), new { id = createdMovimientInventory.Id }, createdMovimientInventory);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear movimiento de inventario");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear movimiento de inventario");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un movimiento de inventario existente
        /// </summary>
        /// <param name="id">ID del movimiento de inventario a actualizar</param>
        /// <param name="movimientInventoryDto">Datos actualizados del movimiento de inventario</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Movimiento de inventario actualizado correctamente</response>
        /// <response code="400">Datos del movimiento de inventario no válidos</response>
        /// <response code="404">Movimiento de inventario no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateMovimientInventory(int id, [FromBody] MovimientInventoryDto movimientInventoryDto)
        {
            if (id != movimientInventoryDto.Id)
            {
                return BadRequest(new { message = "El ID del movimiento de inventario no coincide con el ID proporcionado en el cuerpo de la solicitud." });
            }

            try
            {
                var result = await _movimientInventoryBusiness.UpdateMovimientInventoryAsync(movimientInventoryDto);
                return Ok(new { message = "Movimiento de inventario actualizado correctamente", success = result });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar el movimiento de inventario con ID: {MovimientInventoryId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Movimiento de inventario no encontrado con ID: {MovimientInventoryId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el movimiento de inventario con ID: {MovimientInventoryId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza parcialmente los campos de un movimiento de inventario existente
        /// </summary>
        /// <param name="id">ID del movimiento de inventario a actualizar</param>
        /// <param name="updatedFields">Campos a actualizar</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Movimiento de inventario actualizado parcialmente correctamente</response>
        /// <response code="400">Datos no válidos</response>
        /// <response code="404">Movimiento de inventario no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePartialMovimientInventory(int id, [FromBody] MovimientInventoryDto updatedFields)
        {
            if (updatedFields == null)
            {
                return BadRequest(new { message = "Los datos proporcionados no pueden ser nulos." });
            }

            try
            {
                var result = await _movimientInventoryBusiness.UpdatePartialMovimientInventoryAsync(id, updatedFields);
                return Ok(new { message = "Movimiento de inventario actualizado parcialmente correctamente", success = result });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar parcialmente el movimiento de inventario con ID: {MovimientInventoryId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Movimiento de inventario no encontrado con ID: {MovimientInventoryId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente el movimiento de inventario con ID: {MovimientInventoryId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un movimiento de inventario lógicamente (lo marca como inactivo)
        /// </summary>
        /// <param name="id">ID del movimiento de inventario a eliminar lógicamente</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Movimiento de inventario eliminado lógicamente</response>
        /// <response code="404">Movimiento de inventario no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SoftDeleteMovimientInventory(int id)
        {
            try
            {
                var result = await _movimientInventoryBusiness.SoftDeleteMovimientInventoryAsync(id);
                return Ok(new { message = "Movimiento de inventario eliminado lógicamente", success = result });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Movimiento de inventario no encontrado con ID: {MovimientInventoryId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el movimiento de inventario con ID: {MovimientInventoryId} lógicamente", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un movimiento de inventario permanentemente del sistema
        /// </summary>
        /// <param name="id">ID del movimiento de inventario a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Movimiento de inventario eliminado permanentemente</response>
        /// <response code="404">Movimiento de inventario no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("permanent/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteMovimientInventory(int id)
        {
            try
            {
                var result = await _movimientInventoryBusiness.DeleteMovimientInventoryAsync(id);
                return Ok(new { message = "Movimiento de inventario eliminado permanentemente", success = result });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Movimiento de inventario no encontrado con ID: {MovimientInventoryId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el movimiento de inventario con ID: {MovimientInventoryId} permanentemente", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
