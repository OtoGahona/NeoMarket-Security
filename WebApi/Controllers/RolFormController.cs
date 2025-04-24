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
    /// Controlador para la gestión de formularios de roles en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RolFormController : ControllerBase
    {
        private readonly RolFormBusiness _rolFormBusiness;
        private readonly ILogger<RolFormController> _logger;

        /// <summary>
        /// Constructor del controlador de formularios de roles
        /// </summary>
        /// <param name="rolFormBusiness">Capa de negocio de formularios de roles</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public RolFormController(RolFormBusiness rolFormBusiness, ILogger<RolFormController> logger)
        {
            _rolFormBusiness = rolFormBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los formularios de roles del sistema
        /// </summary>
        /// <returns>Lista de formularios de roles</returns>
        /// <response code="200">Retorna la lista de formularios de roles</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RolFormDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllRolForms()
        {
            try
            {
                var rolForms = await _rolFormBusiness.GetAllRolFormsAsync();
                return Ok(rolForms);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener formularios de roles");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un formulario de rol específico por su ID
        /// </summary>
        /// <param name="id">ID del formulario de rol</param>
        /// <returns>Formulario de rol solicitado</returns>
        /// <response code="200">Retorna el formulario de rol solicitado</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Formulario de rol no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RolFormDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRolFormById(int id)
        {
            try
            {
                var rolForm = await _rolFormBusiness.GetRolFormByIdAsync(id);
                return Ok(rolForm);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el formulario de rol con ID: {RolFormId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Formulario de rol no encontrado con ID: {RolFormId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener formulario de rol con ID: {RolFormId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo formulario de rol en el sistema
        /// </summary>
        /// <param name="rolFormDto">Datos del formulario de rol a crear</param>
        /// <returns>Formulario de rol creado</returns>
        /// <response code="201">Retorna el formulario de rol creado</response>
        /// <response code="400">Datos del formulario de rol no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(RolFormDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateRolForm([FromBody] RolFormDto rolFormDto)
        {
            try
            {
                var createdRolForm = await _rolFormBusiness.CreateRolFormAsync(rolFormDto);
                return CreatedAtAction(nameof(GetRolFormById), new { id = createdRolForm.Id }, createdRolForm);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear formulario de rol");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear formulario de rol");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un rol-formulario existente en el sistema
        /// </summary>
        /// <param name="id">ID del rol-formulario a actualizar</param>
        /// <param name="rolFormDto">Datos actualizados del rol-formulario</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Rol-Formulario actualizado correctamente</response>
        /// <response code="400">Datos del rol-formulario no válidos</response>
        /// <response code="404">Rol-Formulario no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateRolForm(int id, [FromBody] RolFormDto rolFormDto)
        {
            if (id != rolFormDto.Id)
            {
                return BadRequest(new { message = "El ID del rol-formulario no coincide con el ID proporcionado en el cuerpo de la solicitud." });
            }

            try
            {
                var result = await _rolFormBusiness.UpdateRolFormAsync(rolFormDto);
                return Ok(new { message = "Rol-Formulario actualizado correctamente", success = result });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar el rol-formulario con ID: {RolFormId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Rol-Formulario no encontrado con ID: {RolFormId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el rol-formulario con ID: {RolFormId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza campos específicos de un rol-formulario
        /// </summary>
        /// <param name="id">ID del rol-formulario a actualizar</param>
        /// <param name="updatedFields">Campos a actualizar</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Rol-Formulario actualizado correctamente</response>
        /// <response code="400">Datos no válidos</response>
        /// <response code="404">Rol-Formulario no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePartialRolForm(int id, [FromBody] RolFormDto updatedFields)
        {
            if (updatedFields == null)
            {
                return BadRequest(new { message = "Los datos proporcionados no pueden ser nulos." });
            }

            try
            {
                var result = await _rolFormBusiness.UpdatePartialRolFormAsync(id, updatedFields);
                return Ok(new { message = "Rol-Formulario actualizado parcialmente correctamente", success = result });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar parcialmente el rol-formulario con ID: {RolFormId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Rol-Formulario no encontrado con ID: {RolFormId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente el rol-formulario con ID: {RolFormId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Realiza una eliminación lógica de un rol-formulario (marca como inactivo)
        /// </summary>
        /// <param name="id">ID del rol-formulario a eliminar lógicamente</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Rol-Formulario marcado como inactivo correctamente</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Rol-Formulario no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("soft-delete/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SoftDeleteRolForm(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID del rol-formulario debe ser mayor a 0." });
            }

            try
            {
                var result = await _rolFormBusiness.SoftDeleteRolFormAsync(id);
                return Ok(new { message = "Rol-Formulario marcado como inactivo correctamente", success = result });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Rol-Formulario no encontrado con ID: {RolFormId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al realizar la eliminación lógica del rol-formulario con ID: {RolFormId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un rol-formulario del sistema
        /// </summary>
        /// <param name="id">ID del rol-formulario a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Rol-Formulario eliminado correctamente</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Rol-Formulario no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteRolForm(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID del rol-formulario debe ser mayor a 0." });
            }

            try
            {
                var result = await _rolFormBusiness.DeleteRolFormAsync(id);
                return Ok(new { message = "Rol-Formulario eliminado correctamente", success = result });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Rol-Formulario no encontrado con ID: {RolFormId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el rol-formulario con ID: {RolFormId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
