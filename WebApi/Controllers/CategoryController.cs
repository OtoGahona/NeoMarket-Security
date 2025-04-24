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
    /// Controlador para la gestión de categorías en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryBusiness _categoryBusiness;
        private readonly ILogger<CategoryController> _logger;

        /// <summary>
        /// Constructor del controlador de categorías
        /// </summary>
        /// <param name="categoryBusiness">Capa de negocio de categorías</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public CategoryController(CategoryBusiness categoryBusiness, ILogger<CategoryController> logger)
        {
            _categoryBusiness = categoryBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las categorías del sistema
        /// </summary>
        /// <returns>Lista de categorías</returns>
        /// <response code="200">Retorna la lista de categorías</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoryDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _categoryBusiness.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener categorías");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una categoría específica por su ID
        /// </summary>
        /// <param name="id">ID de la categoría</param>
        /// <returns>Categoría solicitada</returns>
        /// <response code="200">Retorna la categoría solicitada</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Categoría no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CategoryDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var category = await _categoryBusiness.GetCategoryByIdAsync(id);
                return Ok(category);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para la categoría con ID: {CategoryId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Categoría no encontrada con ID: {CategoryId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener categoría con ID: {CategoryId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva categoría en el sistema
        /// </summary>
        /// <param name="categoryDto">Datos de la categoría a crear</param>
        /// <returns>Categoría creada</returns>
        /// <response code="201">Retorna la categoría creada</response>
        /// <response code="400">Datos de la categoría no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(CategoryDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDto categoryDto)
        {
            try
            {
                var createdCategory = await _categoryBusiness.CreateCategoryAsync(categoryDto);
                return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Id }, createdCategory);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear categoría");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear categoría");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza una categoría existente en el sistema.
        /// </summary>
        /// <param name="id">ID de la categoría a actualizar</param>
        /// <param name="categoryDto">Datos actualizados de la categoría</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Categoría actualizada correctamente</response>
        /// <response code="400">Datos de la categoría no válidos</response>
        /// <response code="404">Categoría no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDto categoryDto)
        {
            if (id != categoryDto.Id)
            {
                return BadRequest(new { message = "El ID de la categoría no coincide con el ID proporcionado en el cuerpo de la solicitud." });
            }

            try
            {
                var result = await _categoryBusiness.UpdateCategoryAsync(categoryDto);
                return Ok(new { message = "Categoría actualizada correctamente", success = result });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar la categoría con ID: {CategoryId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Categoría no encontrada con ID: {CategoryId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar la categoría con ID: {CategoryId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza campos específicos de una categoría existente en el sistema.
        /// </summary>
        /// <param name="id">ID de la categoría a actualizar</param>
        /// <param name="updatedFields">Campos a actualizar</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Categoría actualizada correctamente</response>
        /// <response code="400">Datos no válidos</response>
        /// <response code="404">Categoría no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePartialCategory(int id, [FromBody] CategoryDto updatedFields)
        {
            if (updatedFields == null)
            {
                return BadRequest(new { message = "Los datos proporcionados no pueden ser nulos." });
            }

            try
            {
                var result = await _categoryBusiness.UpdatePartialCategoryAsync(id, updatedFields);
                return Ok(new { message = "Categoría actualizada parcialmente correctamente", success = result });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar parcialmente la categoría con ID: {CategoryId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Categoría no encontrada con ID: {CategoryId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente la categoría con ID: {CategoryId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Realiza una eliminación lógica de una categoría (marca como inactiva).
        /// </summary>
        /// <param name="id">ID de la categoría a eliminar lógicamente</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Categoría marcada como inactiva correctamente</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Categoría no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("soft-delete/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SoftDeleteCategory(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID de la categoría debe ser mayor a 0." });
            }

            try
            {
                var result = await _categoryBusiness.SoftDeleteCategoryAsync(id);
                return Ok(new { message = "Categoría marcada como inactiva correctamente", success = result });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Categoría no encontrada con ID: {CategoryId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al realizar la eliminación lógica de la categoría con ID: {CategoryId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina una categoría del sistema.
        /// </summary>
        /// <param name="id">ID de la categoría a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Categoría eliminada correctamente</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Categoría no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID de la categoría debe ser mayor a 0." });
            }

            try
            {
                var result = await _categoryBusiness.DeleteCategoryAsync(id);
                return Ok(new { message = "Categoría eliminada correctamente", success = result });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Categoría no encontrada con ID: {CategoryId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar la categoría con ID: {CategoryId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
