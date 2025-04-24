using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business;

/// <summary>
/// Clase de negocio encargada de la lógica relacionada con los elementos de imagen (ImageItem).
/// </summary>
public class ImageItemBusiness
{
    private readonly ImageItemData _imageItemData;
    private readonly ILogger<ImageItemBusiness> _logger;

    public ImageItemBusiness(ImageItemData imageItemData, ILogger<ImageItemBusiness> logger)
    {
        _imageItemData = imageItemData;
        _logger = logger;
    }

    // Método para obtener todos los ImageItems como DTOs
    public async Task<IEnumerable<ImageItemDTO>> GetAllImageItemsAsync()
    {
        try
        {
            var imageItems = await _imageItemData.GetAllAsync();
            return MapToDTOList(imageItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los ImageItems");
            throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de ImageItems", ex);
        }
    }

    // Método para obtener un ImageItem por ID como DTO
    public async Task<ImageItemDTO> GetImageItemByIdAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó obtener un ImageItem con ID inválido: {ImageItemId}", id);
            throw new Utilities.Exceptions.ValidationException("id", "El ID del ImageItem debe ser mayor que cero");
        }

        try
        {
            var imageItem = await _imageItemData.GetByIdAsync(id);
            if (imageItem == null)
            {
                _logger.LogInformation("No se encontró ningún ImageItem con ID: {ImageItemId}", id);
                throw new EntityNotFoundException("ImageItem", id);
            }

            return MapToDTO(imageItem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el ImageItem con ID: {ImageItemId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al recuperar el ImageItem con ID {id}", ex);
        }
    }

    // Método para crear un ImageItem desde un DTO
    public async Task<ImageItemDTO> CreateImageItemAsync(ImageItemDTO imageItemDto)
    {
        try
        {
            ValidateImageItem(imageItemDto);

            var imageItem = MapToEntity(imageItemDto);

            var imageItemCreado = await _imageItemData.CreateAsync(imageItem);
            return MapToDTO(imageItemCreado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear nuevo ImageItem: {ImageItemName}", imageItemDto?.UrlImage ?? "null");
            throw new ExternalServiceException("Base de datos", "Error al crear el ImageItem", ex);
        }
    }

    // Método para actualizar un ImageItem existente
    public async Task<bool> UpdateImageItemAsync(int id, ImageItemDTO imageItemDto)
    {
        try
        {
            ValidateImageItem(imageItemDto);

            var imageItem = MapToEntity(imageItemDto);

            var result = await _imageItemData.UpdateAsync(imageItem);

            if (!result)
            {
                _logger.LogWarning("No se pudo actualizar el ImageItem con ID {ImageItemId}", imageItemDto.Id);
                throw new EntityNotFoundException("ImageItem", imageItemDto.Id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el ImageItem con ID {ImageItemId}", imageItemDto.Id);
            throw new ExternalServiceException("Base de datos", $"Error al actualizar el ImageItem con ID {imageItemDto.Id}", ex);
        }
    }

    // Método para actualizar campos específicos de un ImageItem
    public async Task<bool> UpdatePartialImageItemAsync(int id, ImageItemDTO updatedFields)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó actualizar un ImageItem con un ID inválido: {ImageItemId}", id);
            throw new ValidationException("id", "El ID del ImageItem debe ser mayor a 0");
        }

        try
        {
            // Obtener el ImageItem existente
            var existingImageItem = await _imageItemData.GetByIdAsync(id);
            if (existingImageItem == null)
            {
                _logger.LogInformation("No se encontró el ImageItem con ID {ImageItemId} para actualización parcial", id);
                throw new EntityNotFoundException("ImageItem", id);
            }

            // Actualizar solo los campos proporcionados en el DTO
            if (!string.IsNullOrWhiteSpace(updatedFields.UrlImage))
            {
                existingImageItem.UrlImage = updatedFields.UrlImage;
            }
            if (updatedFields.Status != existingImageItem.Status) // Si Status es un booleano, siempre se envía un valor
            {
                existingImageItem.Status = updatedFields.Status;
            }

            // Guardar los cambios
            var result = await _imageItemData.UpdateAsync(existingImageItem);

            if (!result)
            {
                _logger.LogWarning("No se pudo actualizar parcialmente el ImageItem con ID {ImageItemId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el ImageItem con ID {id}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar parcialmente el ImageItem con ID {ImageItemId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el ImageItem con ID {id}", ex);
        }
    }

    // Método para realizar una eliminación lógica de un ImageItem (marcar como inactivo)
    public async Task<bool> SoftDeleteImageItemAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó realizar una eliminación lógica con un ID inválido: {ImageItemId}", id);
            throw new Utilities.Exceptions.ValidationException("id", "El ID del ImageItem debe ser mayor a 0");
        }

        try
        {
            // Obtener el ImageItem por ID
            var imageItem = await _imageItemData.GetByIdAsync(id);
            if (imageItem == null)
            {
                _logger.LogInformation("No se encontró el ImageItem con ID {ImageItemId} para eliminación lógica", id);
                throw new EntityNotFoundException("ImageItem", id);
            }

            // Marcar el ImageItem como inactivo
            imageItem.Status = false;

            // Actualizar el ImageItem en la base de datos
            var result = await _imageItemData.UpdateAsync(imageItem);

            if (!result)
            {
                _logger.LogWarning("No se pudo realizar la eliminación lógica del ImageItem con ID {ImageItemId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica del ImageItem con ID {id}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al realizar la eliminación lógica del ImageItem con ID {ImageItemId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica del ImageItem con ID {id}", ex);
        }
    }

    // Método para eliminar un ImageItem por su ID
    public async Task<bool> DeleteImageItemAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó eliminar un ImageItem con un ID inválido: {ImageItemId}", id);
            throw new Utilities.Exceptions.ValidationException("id", "El ID del ImageItem debe ser mayor a 0");
        }

        try
        {
            var result = await _imageItemData.DeleteAsync(id);

            if (!result)
            {
                _logger.LogInformation("No se encontró el ImageItem con ID {ImageItemId} para eliminar", id);
                throw new EntityNotFoundException("ImageItem", id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el ImageItem con ID {ImageItemId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al eliminar el ImageItem con ID {id}", ex);
        }
    }

    // Método para validar el DTO
    private void ValidateImageItem(ImageItemDTO imageItemDto)
    {
        if (imageItemDto == null)
        {
            throw new Utilities.Exceptions.ValidationException("El objeto ImageItem no puede ser nulo");
        }

        if (string.IsNullOrWhiteSpace(imageItemDto.UrlImage))
        {
            _logger.LogWarning("Se intentó crear/actualizar un ImageItem con Name vacío");
            throw new Utilities.Exceptions.ValidationException("Name", "El Name del ImageItem es obligatorio");
        }
    }

    // Método para mapear de ImageItem a ImageItemDTO
    private ImageItemDTO MapToDTO(ImageItem imageItem)
    {
        return new ImageItemDTO
        {
            Id = imageItem.Id,
            UrlImage = imageItem.UrlImage,
            Status = imageItem.Status
        };
    }

    // Método para mapear de ImageItemDTO a ImageItem
    private ImageItem MapToEntity(ImageItemDTO imageItemDto)
    {
        return new ImageItem
        {
            Id = imageItemDto.Id,
            UrlImage = imageItemDto.UrlImage,
            Status = imageItemDto.Status
        };
    }

    // Método para mapear una lista de ImageItem a una lista de ImageItemDTO
    private IEnumerable<ImageItemDTO> MapToDTOList(IEnumerable<ImageItem> imageItems)
    {
        var imageItemDtos = new List<ImageItemDTO>();
        foreach (var imageItem in imageItems)
        {
            imageItemDtos.Add(MapToDTO(imageItem));
        }
        return imageItemDtos;
    }

    public async Task UpdateImageItemAsync(ImageItemDTO imageItemDto)
    {
        throw new NotImplementedException();
    }
}
