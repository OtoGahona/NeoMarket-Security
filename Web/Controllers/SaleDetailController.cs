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
    /// Controlador para la gestión de detalles de ventas en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class SaleDetailController : ControllerBase
    {
        private readonly SaleDetailBusiness _saleDetailBusiness;
        private readonly ILogger<SaleDetailController> _logger;

        /// <summary>
        /// Constructor del controlador de detalles de ventas
        /// </summary>
        /// <param name="saleDetailBusiness">Capa de negocio de detalles de ventas</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public SaleDetailController(SaleDetailBusiness saleDetailBusiness, ILogger<SaleDetailController> logger)
        {
            _saleDetailBusiness = saleDetailBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los detalles de ventas del sistema
        /// </summary>
        /// <returns>Lista de detalles de ventas</returns>
        /// <response code="200">Retorna la lista de detalles de ventas</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SaleDetailDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllSaleDetails()
        {
            try
            {
                var saleDetails = await _saleDetailBusiness.GetAllSeleDetailsAsync();
                return Ok(saleDetails);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener detalles de ventas");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un detalle de venta específico por su ID
        /// </summary>
        /// <param name="id">ID del detalle de venta</param>
        /// <returns>Detalle de venta solicitado</returns>
        /// <response code="200">Retorna el detalle de venta solicitado</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Detalle de venta no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SaleDetailDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetSaleDetailById(int id)
        {
            try
            {
                var saleDetail = await _saleDetailBusiness.GetSeleDetailByIdAsync(id);
                return Ok(saleDetail);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el detalle de venta con ID: {SaleDetailId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Detalle de venta no encontrado con ID: {SaleDetailId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener detalle de venta con ID: {SaleDetailId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo detalle de venta en el sistema
        /// </summary>
        /// <param name="saleDetailDto">Datos del detalle de venta a crear</param>
        /// <returns>Detalle de venta creado</returns>
        /// <response code="201">Retorna el detalle de venta creado</response>
        /// <response code="400">Datos del detalle de venta no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(SaleDetailDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateSaleDetail([FromBody] SaleDetailDTO saleDetailDto)
        {
            try
            {
                var createdSaleDetail = await _saleDetailBusiness.CreateSeleDetailAsync(saleDetailDto);
                return CreatedAtAction(nameof(GetSaleDetailById), new { id = createdSaleDetail.Id }, createdSaleDetail);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear detalle de venta");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear detalle de venta");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un detalle de venta existente en el sistema
        /// </summary>
        /// <param name="id">ID del detalle de venta a actualizar</param>
        /// <param name="saleDetailDto">Datos actualizados del detalle de venta</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Detalle de venta actualizado correctamente</response>
        /// <response code="400">Datos del detalle no válidos</response>
        /// <response code="404">Detalle de venta no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateSaleDetail(int id, [FromBody] SaleDetailDTO saleDetailDto)
        {
            if (id != saleDetailDto.Id)
            {
                return BadRequest(new { message = "El ID del detalle de venta no coincide con el ID proporcionado en el cuerpo de la solicitud." });
            }

            try
            {
                var result = await _saleDetailBusiness.UpdateSaleDetailAsync(saleDetailDto);
                return Ok(new { message = "Detalle de venta actualizado correctamente", success = result });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar el detalle con ID: {SaleDetailId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Detalle de venta no encontrado con ID: {SaleDetailId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el detalle con ID: {SaleDetailId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza campos específicos de un detalle de venta
        /// </summary>
        /// <param name="id">ID del detalle de venta a actualizar</param>
        /// <param name="updatedFields">Campos a actualizar</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Detalle actualizado correctamente</response>
        /// <response code="400">Datos no válidos</response>
        /// <response code="404">Detalle no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePartialSaleDetail(int id, [FromBody] SaleDetailDTO updatedFields)
        {
            if (updatedFields == null)
            {
                return BadRequest(new { message = "Los datos proporcionados no pueden ser nulos." });
            }

            try
            {
                var result = await _saleDetailBusiness.UpdatePartialSaleDetailAsync(id, updatedFields);
                return Ok(new { message = "Detalle de venta actualizado parcialmente correctamente", success = result });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar parcialmente el detalle con ID: {SaleDetailId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Detalle de venta no encontrado con ID: {SaleDetailId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente el detalle con ID: {SaleDetailId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Realiza una eliminación lógica de un detalle de venta (marca como inactivo)
        /// </summary>
        /// <param name="id">ID del detalle de venta a eliminar lógicamente</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Detalle de venta marcado como inactivo correctamente</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Detalle no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("soft-delete/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SoftDeleteSaleDetail(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID del detalle de venta debe ser mayor a 0." });
            }

            try
            {
                var result = await _saleDetailBusiness.SoftDeleteSaleDetailAsync(id);
                return Ok(new { message = "Detalle de venta marcado como inactivo correctamente", success = result });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Detalle de venta no encontrado con ID: {SaleDetailId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al realizar la eliminación lógica del detalle con ID: {SaleDetailId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un detalle de venta del sistema
        /// </summary>
        /// <param name="id">ID del detalle de venta a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Detalle eliminado correctamente</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Detalle no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteSaleDetail(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID del detalle de venta debe ser mayor a 0." });
            }

            try
            {
                var result = await _saleDetailBusiness.DeleteSaleDetailAsync(id);
                return Ok(new { message = "Detalle de venta eliminado correctamente", success = result });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Detalle de venta no encontrado con ID: {SaleDetailId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el detalle con ID: {SaleDetailId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
