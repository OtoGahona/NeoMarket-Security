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
    /// Controlador para la gestión de ventas en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class SaleController : ControllerBase
    {
        private readonly SaleBusiness _saleBusiness;
        private readonly ILogger<SaleController> _logger;

        /// <summary>
        /// Constructor del controlador de ventas
        /// </summary>
        /// <param name="saleBusiness">Capa de negocio de ventas</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public SaleController(SaleBusiness saleBusiness, ILogger<SaleController> logger)
        {
            _saleBusiness = saleBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las ventas del sistema
        /// </summary>
        /// <returns>Lista de ventas</returns>
        /// <response code="200">Retorna la lista de ventas</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SaleDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllSales()
        {
            try
            {
                var sales = await _saleBusiness.GetAllSeleAsync();
                return Ok(sales);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener ventas");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una venta específica por su ID
        /// </summary>
        /// <param name="id">ID de la venta</param>
        /// <returns>Venta solicitada</returns>
        /// <response code="200">Retorna la venta solicitada</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Venta no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SaleDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetSaleById(int id)
        {
            try
            {
                var sale = await _saleBusiness.GetSeleByIdAsync(id);
                return Ok(sale);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para la venta con ID: {SaleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Venta no encontrada con ID: {SaleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener venta con ID: {SaleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva venta en el sistema
        /// </summary>
        /// <param name="saleDto">Datos de la venta a crear</param>
        /// <returns>Venta creada</returns>
        /// <response code="201">Retorna la venta creada</response>
        /// <response code="400">Datos de la venta no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(SaleDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateSale([FromBody] SaleDTO saleDto)
        {
            try
            {
                var createdSale = await _saleBusiness.CreateSeleAsync(saleDto);
                return CreatedAtAction(nameof(GetSaleById), new { id = createdSale.Id }, createdSale);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear venta");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear venta");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza una venta existente en el sistema
        /// </summary>
        /// <param name="id">ID de la venta a actualizar</param>
        /// <param name="saleDto">Datos actualizados de la venta</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Venta actualizada correctamente</response>
        /// <response code="400">Datos de la venta no válidos</response>
        /// <response code="404">Venta no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateSale(int id, [FromBody] SaleDTO saleDto)
        {
            if (id != saleDto.Id)
            {
                return BadRequest(new { message = "El ID de la venta no coincide con el ID proporcionado en el cuerpo de la solicitud." });
            }

            try
            {
                var result = await _saleBusiness.UpdateSaleAsync(saleDto);
                return Ok(new { message = "Venta actualizada correctamente", success = result });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar la venta con ID: {SaleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Venta no encontrada con ID: {SaleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar la venta con ID: {SaleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza campos específicos de una venta
        /// </summary>
        /// <param name="id">ID de la venta a actualizar</param>
        /// <param name="updatedFields">Campos a actualizar</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Venta actualizada correctamente</response>
        /// <response code="400">Datos no válidos</response>
        /// <response code="404">Venta no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePartialSale(int id, [FromBody] SaleDTO updatedFields)
        {
            if (updatedFields == null)
            {
                return BadRequest(new { message = "Los datos proporcionados no pueden ser nulos." });
            }

            try
            {
                var result = await _saleBusiness.UpdatePartialSaleAsync(id, updatedFields);
                return Ok(new { message = "Venta actualizada parcialmente correctamente", success = result });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar parcialmente la venta con ID: {SaleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Venta no encontrada con ID: {SaleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente la venta con ID: {SaleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Realiza una eliminación lógica de una venta (marca como inactiva)
        /// </summary>
        /// <param name="id">ID de la venta a eliminar lógicamente</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Venta marcada como inactiva correctamente</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Venta no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("soft-delete/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SoftDeleteSale(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID de la venta debe ser mayor a 0." });
            }

            try
            {
                var result = await _saleBusiness.SoftDeleteSaleAsync(id);
                return Ok(new { message = "Venta marcada como inactiva correctamente", success = result });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Venta no encontrada con ID: {SaleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al realizar la eliminación lógica de la venta con ID: {SaleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina una venta del sistema
        /// </summary>
        /// <param name="id">ID de la venta a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Venta eliminada correctamente</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Venta no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteSale(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID de la venta debe ser mayor a 0." });
            }

            try
            {
                var result = await _saleBusiness.DeleteSaleAsync(id);
                return Ok(new { message = "Venta eliminada correctamente", success = result });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Venta no encontrada con ID: {SaleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar la venta con ID: {SaleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
