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
    /// Controlador para la gestión de sedes en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class SedeController : ControllerBase
    {
        private readonly SedeBusiness _sedeBusiness;
        private readonly ILogger<SedeController> _logger;

        /// <summary>
        /// Constructor del controlador de sedes
        /// </summary>
        /// <param name="sedeBusiness">Capa de negocio de sedes</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public SedeController(SedeBusiness sedeBusiness, ILogger<SedeController> logger)
        {
            _sedeBusiness = sedeBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las sedes del sistema
        /// </summary>
        /// <returns>Lista de sedes</returns>
        /// <response code="200">Retorna la lista de sedes</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SedeDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllSedes()
        {
            try
            {
                var sedes = await _sedeBusiness.GetAllSedesAsync();
                return Ok(sedes);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener sedes");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una sede específica por su ID
        /// </summary>
        /// <param name="id">ID de la sede</param>
        /// <returns>Sede solicitada</returns>
        /// <response code="200">Retorna la sede solicitada</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Sede no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SedeDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetSedeById(int id)
        {
            try
            {
                var sede = await _sedeBusiness.GetSedeByIdAsync(id);
                return Ok(sede);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para la sede con ID: {SedeId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Sede no encontrada con ID: {SedeId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener sede con ID: {SedeId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva sede en el sistema
        /// </summary>
        /// <param name="sedeDto">Datos de la sede a crear</param>
        /// <returns>Sede creada</returns>
        /// <response code="201">Retorna la sede creada</response>
        /// <response code="400">Datos de la sede no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(SedeDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateSede([FromBody] SedeDto sedeDto)
        {
            try
            {
                var createdSede = await _sedeBusiness.CreateSedeAsync(sedeDto);
                return CreatedAtAction(nameof(GetSedeById), new { id = createdSede.Id }, createdSede);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear sede");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear sede");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza una sede existente en el sistema
        /// </summary>
        /// <param name="id">ID de la sede a actualizar</param>
        /// <param name="sedeDto">Datos actualizados de la sede</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Sede actualizada correctamente</response>
        /// <response code="400">Datos de la sede no válidos</response>
        /// <response code="404">Sede no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateSede(int id, [FromBody] SedeDto sedeDto)
        {
            if (id != sedeDto.Id)
            {
                return BadRequest(new { message = "El ID de la sede no coincide con el ID proporcionado en el cuerpo de la solicitud." });
            }

            try
            {
                var result = await _sedeBusiness.UpdateSedeAsync(sedeDto);
                return Ok(new { message = "Sede actualizada correctamente", success = result });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar la sede con ID: {SedeId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Sede no encontrada con ID: {SedeId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar la sede con ID: {SedeId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza campos específicos de una sede
        /// </summary>
        /// <param name="id">ID de la sede a actualizar</param>
        /// <param name="updatedFields">Campos a actualizar</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Sede actualizada correctamente</response>
        /// <response code="400">Datos no válidos</response>
        /// <response code="404">Sede no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePartialSede(int id, [FromBody] SedeDto updatedFields)
        {
            if (updatedFields == null)
            {
                return BadRequest(new { message = "Los datos proporcionados no pueden ser nulos." });
            }

            try
            {
                var result = await _sedeBusiness.UpdatePartialSedeAsync(id, updatedFields);
                return Ok(new { message = "Sede actualizada parcialmente correctamente", success = result });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar parcialmente la sede con ID: {SedeId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Sede no encontrada con ID: {SedeId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente la sede con ID: {SedeId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Realiza una eliminación lógica de una sede (marca como inactiva)
        /// </summary>
        /// <param name="id">ID de la sede a eliminar lógicamente</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Sede marcada como inactiva correctamente</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Sede no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("soft-delete/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SoftDeleteSede(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID de la sede debe ser mayor a 0." });
            }

            try
            {
                var result = await _sedeBusiness.SoftDeleteSedeAsync(id);
                return Ok(new { message = "Sede marcada como inactiva correctamente", success = result });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Sede no encontrada con ID: {SedeId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al realizar la eliminación lógica de la sede con ID: {SedeId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina una sede del sistema
        /// </summary>
        /// <param name="id">ID de la sede a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Sede eliminada correctamente</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Sede no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteSede(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID de la sede debe ser mayor a 0." });
            }

            try
            {
                var result = await _sedeBusiness.DeleteSedeAsync(id);
                return Ok(new { message = "Sede eliminada correctamente", success = result });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Sede no encontrada con ID: {SedeId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar la sede con ID: {SedeId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
