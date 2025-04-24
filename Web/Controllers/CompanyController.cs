using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de empresas en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class CompanyController : ControllerBase
    {
        private readonly CompanyBusiness _companyBusiness;
        private readonly ILogger<CompanyController> _logger;

        /// <summary>
        /// Constructor del controlador de empresas
        /// </summary>
        /// <param name="companyBusiness">Capa de negocio de empresas</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public CompanyController(CompanyBusiness companyBusiness, ILogger<CompanyController> logger)
        {
            _companyBusiness = companyBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las empresas del sistema
        /// </summary>
        /// <returns>Lista de empresas</returns>
        /// <response code="200">Retorna la lista de empresas</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CompanyDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllCompanies()
        {
            try
            {
                var companies = await _companyBusiness.GetAllCompaniesAsync();
                return Ok(companies);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener empresas");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una empresa específica por su ID
        /// </summary>
        /// <param name="id">ID de la empresa</param>
        /// <returns>Empresa solicitada</returns>
        /// <response code="200">Retorna la empresa solicitada</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Empresa no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CompanyDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetCompanyById(int id)
        {
            try
            {
                var company = await _companyBusiness.GetCompanyByIdAsync(id);
                return Ok(company);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para la empresa con ID: {CompanyId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Empresa no encontrada con ID: {CompanyId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener empresa con ID: {CompanyId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva empresa en el sistema
        /// </summary>
        /// <param name="companyDto">Datos de la empresa a crear</param>
        /// <returns>Empresa creada</returns>
        /// <response code="201">Retorna la empresa creada</response>
        /// <response code="400">Datos de la empresa no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(CompanyDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyDto companyDto)
        {
            try
            {
                var createdCompany = await _companyBusiness.CreateCompanyAsync(companyDto);
                return CreatedAtAction(nameof(GetCompanyById), new { id = createdCompany.Id }, createdCompany);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear empresa");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear empresa");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza una empresa existente en el sistema.
        /// </summary>
        /// <param name="id">ID de la empresa a actualizar</param>
        /// <param name="companyDto">Datos actualizados de la empresa</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Empresa actualizada correctamente</response>
        /// <response code="400">Datos de la empresa no válidos</response>
        /// <response code="404">Empresa no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateCompany(int id, [FromBody] CompanyDto companyDto)
        {
            if (id != companyDto.Id)
            {
                return BadRequest(new { message = "El ID de la empresa no coincide con el ID proporcionado en el cuerpo de la solicitud." });
            }

            try
            {
                var result = await _companyBusiness.UpdateCompanyAsync(companyDto);
                return Ok(new { message = "Empresa actualizada correctamente", success = result });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar la empresa con ID: {CompanyId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Empresa no encontrada con ID: {CompanyId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar la empresa con ID: {CompanyId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza campos específicos de una empresa existente en el sistema.
        /// </summary>
        /// <param name="id">ID de la empresa a actualizar</param>
        /// <param name="updatedFields">Campos a actualizar</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Empresa actualizada correctamente</response>
        /// <response code="400">Datos no válidos</response>
        /// <response code="404">Empresa no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePartialCompany(int id, [FromBody] CompanyDto updatedFields)
        {
            if (updatedFields == null)
            {
                return BadRequest(new { message = "Los datos proporcionados no pueden ser nulos." });
            }

            try
            {
                var result = await _companyBusiness.UpdatePartialCompanyAsync(id, updatedFields);
                return Ok(new { message = "Empresa actualizada parcialmente correctamente", success = result });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar parcialmente la empresa con ID: {CompanyId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Empresa no encontrada con ID: {CompanyId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente la empresa con ID: {CompanyId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Realiza una eliminación lógica de una empresa (marca como inactiva).
        /// </summary>
        /// <param name="id">ID de la empresa a eliminar lógicamente</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Empresa marcada como inactiva correctamente</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Empresa no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("soft-delete/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SoftDeleteCompany(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID de la empresa debe ser mayor a 0." });
            }

            try
            {
                var result = await _companyBusiness.SoftDeleteCompanyAsync(id);
                return Ok(new { message = "Empresa marcada como inactiva correctamente", success = result });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Empresa no encontrada con ID: {CompanyId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al realizar la eliminación lógica de la empresa con ID: {CompanyId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina una empresa del sistema.
        /// </summary>
        /// <param name="id">ID de la empresa a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Empresa eliminada correctamente</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Empresa no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID de la empresa debe ser mayor a 0." });
            }

            try
            {
                var result = await _companyBusiness.DeleteCompanyAsync(id);
                return Ok(new { message = "Empresa eliminada correctamente", success = result });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Empresa no encontrada con ID: {CompanyId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar la empresa con ID: {CompanyId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
