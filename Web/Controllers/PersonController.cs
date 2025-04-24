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
    /// Controlador para la gestión de personas en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class PersonController : ControllerBase
    {
        private readonly PersonBusiness _personBusiness;
        private readonly ILogger<PersonController> _logger;

        /// <summary>
        /// Constructor del controlador de personas
        /// </summary>
        /// <param name="personBusiness">Capa de negocio de personas</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public PersonController(PersonBusiness personBusiness, ILogger<PersonController> logger)
        {
            _personBusiness = personBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las personas del sistema
        /// </summary>
        /// <returns>Lista de personas</returns>
        /// <response code="200">Retorna la lista de personas</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PersonDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllPersons()
        {
            try
            {
                var persons = await _personBusiness.GetAllPersonsAsync();
                return Ok(persons);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener personas");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una persona específica por su ID
        /// </summary>
        /// <param name="id">ID de la persona</param>
        /// <returns>Persona solicitada</returns>
        /// <response code="200">Retorna la persona solicitada</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Persona no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PersonDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetPersonById(int id)
        {
            try
            {
                var person = await _personBusiness.GetPersonByIdAsync(id);
                return Ok(person);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para la persona con ID: {PersonId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Persona no encontrada con ID: {PersonId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener persona con ID: {PersonId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva persona en el sistema
        /// </summary>
        /// <param name="personDto">Datos de la persona a crear</param>
        /// <returns>Persona creada</returns>
        /// <response code="201">Retorna la persona creada</response>
        /// <response code="400">Datos de la persona no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(PersonDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreatePerson([FromBody] PersonDto personDto)
        {
            try
            {
                var createdPerson = await _personBusiness.CreatePersonAsync(personDto);
                return CreatedAtAction(nameof(GetPersonById), new { id = createdPerson.Id }, createdPerson);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear persona");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear persona");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza una persona existente en el sistema
        /// </summary>
        /// <param name="id">ID de la persona a actualizar</param>
        /// <param name="personDto">Datos actualizados de la persona</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Persona actualizada correctamente</response>
        /// <response code="400">Datos de la persona no válidos</response>
        /// <response code="404">Persona no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePerson(int id, [FromBody] PersonDto personDto)
        {
            if (id != personDto.Id)
            {
                return BadRequest(new { message = "El ID de la persona no coincide con el ID proporcionado en el cuerpo de la solicitud." });
            }

            try
            {
                var result = await _personBusiness.UpdatePersonAsync(personDto);
                return Ok(new { message = "Persona actualizada correctamente", success = result });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar la persona con ID: {PersonId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Persona no encontrada con ID: {PersonId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar la persona con ID: {PersonId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza campos específicos de una persona
        /// </summary>
        /// <param name="id">ID de la persona a actualizar</param>
        /// <param name="updatedFields">Campos a actualizar</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Persona actualizada correctamente</response>
        /// <response code="400">Datos no válidos</response>
        /// <response code="404">Persona no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePartialPerson(int id, [FromBody] PersonDto updatedFields)
        {
            if (updatedFields == null)
            {
                return BadRequest(new { message = "Los datos proporcionados no pueden ser nulos." });
            }

            try
            {
                var result = await _personBusiness.UpdatePartialPersonAsync(id, updatedFields);
                return Ok(new { message = "Persona actualizada parcialmente correctamente", success = result });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar parcialmente la persona con ID: {PersonId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Persona no encontrada con ID: {PersonId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente la persona con ID: {PersonId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Realiza una eliminación lógica de una persona (marca como inactiva)
        /// </summary>
        /// <param name="id">ID de la persona a eliminar lógicamente</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Persona marcada como inactiva correctamente</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Persona no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("soft-delete/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SoftDeletePerson(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID de la persona debe ser mayor a 0." });
            }

            try
            {
                var result = await _personBusiness.SoftDeletePersonAsync(id);
                return Ok(new { message = "Persona marcada como inactiva correctamente", success = result });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Persona no encontrada con ID: {PersonId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al realizar la eliminación lógica de la persona con ID: {PersonId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina una persona del sistema
        /// </summary>
        /// <param name="id">ID de la persona a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Persona eliminada correctamente</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Persona no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeletePerson(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID de la persona debe ser mayor a 0." });
            }

            try
            {
                var result = await _personBusiness.DeletePersonAsync(id);
                return Ok(new { message = "Persona eliminada correctamente", success = result });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Persona no encontrada con ID: {PersonId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar la persona con ID: {PersonId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
