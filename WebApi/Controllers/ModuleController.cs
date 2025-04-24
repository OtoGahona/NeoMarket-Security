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
    /// Controlador para la gestión de módulos en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ModuleController : ControllerBase
    {
        private readonly ModuleBusiness _moduleBusiness;
        private readonly ILogger<ModuleController> _logger;

        /// <summary>
        /// Constructor del controlador de módulos
        /// </summary>
        /// <param name="moduleBusiness">Capa de negocio de módulos</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public ModuleController(ModuleBusiness moduleBusiness, ILogger<ModuleController> logger)
        {
            _moduleBusiness = moduleBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los módulos del sistema
        /// </summary>
        /// <returns>Lista de módulos</returns>
        /// <response code="200">Retorna la lista de módulos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ModuleDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllModules()
        {
            try
            {
                var modules = await _moduleBusiness.GetAllModulesAsync();
                return Ok(modules);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener módulos");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un módulo específico por su ID
        /// </summary>
        /// <param name="id">ID del módulo</param>
        /// <returns>Módulo solicitado</returns>
        /// <response code="200">Retorna el módulo solicitado</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Módulo no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ModuleDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetModuleById(int id)
        {
            try
            {
                var module = await _moduleBusiness.GetModuleByIdAsync(id);
                return Ok(module);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el módulo con ID: {ModuleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Módulo no encontrado con ID: {ModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener módulo con ID: {ModuleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo módulo en el sistema
        /// </summary>
        /// <param name="moduleDto">Datos del módulo a crear</param>
        /// <returns>Módulo creado</returns>
        /// <response code="201">Retorna el módulo creado</response>
        /// <response code="400">Datos del módulo no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(ModuleDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateModule([FromBody] ModuleDto moduleDto)
        {
            try
            {
                var createdModule = await _moduleBusiness.CreateModuleAsync(moduleDto);
                return CreatedAtAction(nameof(GetModuleById), new { id = createdModule.Id }, createdModule);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear módulo");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear módulo");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un módulo existente en el sistema
        /// </summary>
        /// <param name="id">ID del módulo a actualizar</param>
        /// <param name="moduleDto">Datos actualizados del módulo</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Módulo actualizado correctamente</response>
        /// <response code="400">Datos del módulo no válidos</response>
        /// <response code="404">Módulo no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateModule(int id, [FromBody] ModuleDto moduleDto)
        {
            if (id != moduleDto.Id)
            {
                return BadRequest(new { message = "El ID del módulo no coincide con el ID proporcionado en el cuerpo de la solicitud." });
            }

            try
            {
                var result = await _moduleBusiness.UpdateModuleAsync(moduleDto);
                return Ok(new { message = "Módulo actualizado correctamente", success = result });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar el módulo con ID: {ModuleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Módulo no encontrado con ID: {ModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el módulo con ID: {ModuleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza parcialmente los campos de un módulo existente
        /// </summary>
        /// <param name="id">ID del módulo a actualizar</param>
        /// <param name="updatedFields">Campos a actualizar</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Módulo actualizado parcialmente correctamente</response>
        /// <response code="400">Datos no válidos</response>
        /// <response code="404">Módulo no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePartialModule(int id, [FromBody] ModuleDto updatedFields)
        {
            if (updatedFields == null)
            {
                return BadRequest(new { message = "Los datos proporcionados no pueden ser nulos." });
            }

            try
            {
                var result = await _moduleBusiness.UpdatePartialModuleAsync(id, updatedFields);
                return Ok(new { message = "Módulo actualizado parcialmente correctamente", success = result });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar parcialmente el módulo con ID: {ModuleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Módulo no encontrado con ID: {ModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente el módulo con ID: {ModuleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un módulo lógicamente (lo marca como inactivo)
        /// </summary>
        /// <param name="id">ID del módulo a eliminar lógicamente</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Módulo eliminado lógicamente</response>
        /// <response code="404">Módulo no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SoftDeleteModule(int id)
        {
            try
            {
                var result = await _moduleBusiness.SoftDeleteModuleAsync(id);
                return Ok(new { message = "Módulo eliminado lógicamente", success = result });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Módulo no encontrado con ID: {ModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el módulo con ID: {ModuleId} lógicamente", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un módulo permanentemente del sistema
        /// </summary>
        /// <param name="id">ID del módulo a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Módulo eliminado permanentemente</response>
        /// <response code="404">Módulo no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("permanent/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteModule(int id)
        {
            try
            {
                var result = await _moduleBusiness.DeleteModuleAsync(id);
                return Ok(new { message = "Módulo eliminado permanentemente", success = result });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Módulo no encontrado con ID: {ModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el módulo con ID: {ModuleId} permanentemente", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
