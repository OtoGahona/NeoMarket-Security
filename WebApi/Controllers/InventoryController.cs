using Business;
using Data;
using Entity.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de inventarios en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryBusiness _inventoryBusiness;
        private readonly ILogger<InventoryController> _logger;

        /// <summary>
        /// Constructor del controlador de inventarios
        /// </summary>
        /// <param name="inventoryBusiness">Capa de negocio de inventarios</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public InventoryController(InventoryBusiness inventoryBusiness, ILogger<InventoryController> logger)
        {
            _inventoryBusiness = inventoryBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los inventarios del sistema
        /// </summary>
        /// <returns>Lista de inventarios</returns>
        /// <response code="200">Retorna la lista de inventarios</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<InventoryDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllInventories()
        {
            try
            {
                var inventories = await _inventoryBusiness.GetAllInventoriesAsync();
                return Ok(inventories);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener inventarios");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un inventario específico por su ID
        /// </summary>
        /// <param name="id">ID del inventario</param>
        /// <returns>Inventario solicitado</returns>
        /// <response code="200">Retorna el inventario solicitado</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Inventario no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(InventoryDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetInventoryById(int id)
        {
            try
            {
                var inventory = await _inventoryBusiness.GetInventoryByIdAsync(id);
                return Ok(inventory);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el inventario con ID: {InventoryId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Inventario no encontrado con ID: {InventoryId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener inventario con ID: {InventoryId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo inventario en el sistema
        /// </summary>
        /// <param name="inventoryDto">Datos del inventario a crear</param>
        /// <returns>Inventario creado</returns>
        /// <response code="201">Retorna el inventario creado</response>
        /// <response code="400">Datos del inventario no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(InventoryDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateInventory([FromBody] InventoryDto inventoryDto)
        {
            try
            {
                var createdInventory = await _inventoryBusiness.CreateInventoryAsync(inventoryDto);
                return CreatedAtAction(nameof(GetInventoryById), new { id = createdInventory.Id }, createdInventory);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear inventario");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear inventario");
                return StatusCode(500, new { message = ex.Message });
            }
        } 
 /// <summary>
/// Actualiza un inventario existente en el sistema
/// </summary>
/// <param name="id">ID del inventario a actualizar</param>
/// <param name="inventoryDto">Datos actualizados del inventario</param>
/// <returns>Resultado de la operación</returns>
/// <response code="200">Inventario actualizado correctamente</response>
/// <response code="400">Datos del inventario no válidos</response>
/// <response code="404">Inventario no encontrado</response>
/// <response code="500">Error interno del servidor</response>
[HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateInventory(int id, [FromBody] InventoryDto inventoryDto)
        {
            if (id != inventoryDto.Id)
            {
                return BadRequest(new { message = "El ID del inventario no coincide con el ID proporcionado en el cuerpo de la solicitud." });
            }

            try
            {
                await _inventoryBusiness.UpdateAsync(inventoryDto);
                return Ok(new { message = "Inventario actualizado correctamente", success = true });

            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar el inventario con ID: {InventoryId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Inventario no encontrado con ID: {InventoryId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el inventario con ID: {InventoryId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza campos específicos de un inventario
        /// </summary>
        /// <param name="id">ID del inventario a actualizar</param>
        /// <param name="updatedFields">Campos a actualizar</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Inventario actualizado correctamente</response>
        /// <response code="400">Datos no válidos</response>
        /// <response code="404">Inventario no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePartialInventory(int id, [FromBody] InventoryDto updatedFields)
        {
            if (updatedFields == null)
            {
                return BadRequest(new { message = "Los datos proporcionados no pueden ser nulos." });
            }

            try
            {
                var result = await _inventoryBusiness.UpdateInventoryAsync(id, updatedFields);
                return Ok(new { message = "Inventario actualizado parcialmente correctamente", success = result });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar parcialmente el inventario con ID: {InventoryId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Inventario no encontrado con ID: {InventoryId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente el inventario con ID: {InventoryId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Realiza una eliminación lógica de un inventario (marca como inactivo)
        /// </summary>
        /// <param name="id">ID del inventario a eliminar lógicamente</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Inventario marcado como inactivo correctamente</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Inventario no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("soft-delete/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SoftDeleteInventory(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID del inventario debe ser mayor a 0." });
            }

            try
            {
                var result = await _inventoryBusiness.SoftDeleteInventoryAsync(id);
                return Ok(new { message = "Inventario marcado como inactivo correctamente", success = result });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Inventario no encontrado con ID: {InventoryId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al realizar la eliminación lógica del inventario con ID: {InventoryId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un inventario del sistema
        /// </summary>
        /// <param name="id">ID del inventario a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Inventario eliminado correctamente</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Inventario no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID del inventario debe ser mayor a 0." });
            }

            try
            {
                var result = await _inventoryBusiness.DeleteInventoryAsync(id);
                return Ok(new { message = "Inventario eliminado correctamente", success = result });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Inventario no encontrado con ID: {InventoryId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el inventario con ID: {InventoryId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}