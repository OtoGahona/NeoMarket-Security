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
    /// Controlador para la gestión de imágenes en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ImageItemController : ControllerBase
    {
        private readonly ImageItemBusiness _imageItemBusiness;
        private readonly ILogger<ImageItemController> _logger;

        /// <summary>
        /// Constructor del controlador de imágenes
        /// </summary>
        /// <param name="imageItemBusiness">Capa de negocio de imágenes</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public ImageItemController(ImageItemBusiness imageItemBusiness, ILogger<ImageItemController> logger)
        {
            _imageItemBusiness = imageItemBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las imágenes del sistema
        /// </summary>
        /// <returns>Lista de imágenes</returns>
        /// <response code="200">Retorna la lista de imágenes</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ImageItemDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllImageItems()
        {
            try
            {
                var imageItems = await _imageItemBusiness.GetAllImageItemsAsync();
                return Ok(imageItems);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener imágenes");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una imagen específica por su ID
        /// </summary>
        /// <param name="id">ID de la imagen</param>
        /// <returns>Imagen solicitada</returns>
        /// <response code="200">Retorna la imagen solicitada</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Imagen no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ImageItemDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetImageItemById(int id)
        {
            try
            {
                var imageItem = await _imageItemBusiness.GetImageItemByIdAsync(id);
                return Ok(imageItem);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para la imagen con ID: {ImageItemId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Imagen no encontrada con ID: {ImageItemId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener imagen con ID: {ImageItemId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva imagen en el sistema
        /// </summary>
        /// <param name="imageItemDto">Datos de la imagen a crear</param>
        /// <returns>Imagen creada</returns>
        /// <response code="201">Retorna la imagen creada</response>
        /// <response code="400">Datos de la imagen no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(ImageItemDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateImageItem([FromBody] ImageItemDTO imageItemDto)
        {
            try
            {
                var createdImageItem = await _imageItemBusiness.CreateImageItemAsync(imageItemDto);
                return CreatedAtAction(nameof(GetImageItemById), new { id = createdImageItem.Id }, createdImageItem);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear imagen");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear imagen");
                return StatusCode(500, new { message = ex.Message });
            }
        }


        /// <summary>
        /// Actualiza parcialmente los campos de una imagen existente
        /// </summary>
        /// <param name="id">ID de la imagen a actualizar</param>
        /// <param name="updatedFields">Campos a actualizar</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Imagen actualizada parcialmente correctamente</response>
        /// <response code="400">Datos no válidos</response>
        /// <response code="404">Imagen no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePartialImageItem(int id, [FromBody] ImageItemDTO updatedFields)
        {
            if (updatedFields == null)
            {
                return BadRequest(new { message = "Los datos proporcionados no pueden ser nulos." });
            }

            try
            {
                var result = await _imageItemBusiness.UpdateImageItemAsync(id, updatedFields);
                return Ok(new { message = "Imagen actualizada parcialmente correctamente", success = result });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar parcialmente la imagen con ID: {ImageItemId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Imagen no encontrada con ID: {ImageItemId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente la imagen con ID: {ImageItemId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un ImageItem existente en el sistema
        /// </summary>
        /// <param name="id">ID del ImageItem a actualizar</param>
        /// <param name="imageItemDto">Datos actualizados del ImageItem</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">ImageItem actualizado correctamente</response>
        /// <response code="400">Datos no válidos</response>
        /// <response code="404">ImageItem no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPut("update/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateImageItemAsync(int id, [FromBody] ImageItemDTO imageItemDto)
        {
            if (id != imageItemDto.Id)
            {
                return BadRequest(new { message = "El ID proporcionado no coincide con el ID del objeto." });
            }

            try
            {
                var result = await _imageItemBusiness.UpdateImageItemAsync(id, imageItemDto);
                return Ok(new { message = "ImageItem actualizado correctamente", success = result });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar el ImageItem con ID: {ImageItemId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "ImageItem no encontrado con ID: {ImageItemId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el ImageItem con ID: {ImageItemId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }



        /// <summary>
        /// Realiza una eliminación lógica de una imagen de item (marca como inactiva)
        /// </summary>
        /// <param name="id">ID de la imagen del item a eliminar lógicamente</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Imagen de item marcada como inactiva correctamente</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Imagen del item no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("soft-delete/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SoftDeleteImageItem(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID de la imagen del item debe ser mayor a 0." });
            }

            try
            {
                var result = await _imageItemBusiness.SoftDeleteImageItemAsync(id);
                return Ok(new { message = "Imagen de item marcada como inactiva correctamente", success = result });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Imagen del item no encontrada con ID: {ImageItemId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al realizar la eliminación lógica de la imagen del item con ID: {ImageItemId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina una imagen de item del sistema
        /// </summary>
        /// <param name="id">ID de la imagen del item a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Imagen de item eliminada correctamente</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Imagen del item no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteImageItem(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID de la imagen del item debe ser mayor a 0." });
            }

            try
            {
                var result = await _imageItemBusiness.DeleteImageItemAsync(id);
                return Ok(new { message = "Imagen de item eliminada correctamente", success = result });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Imagen del item no encontrada con ID: {ImageItemId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar la imagen del item con ID: {ImageItemId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}