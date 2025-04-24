using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business;

/// <summary>
/// Clase de negocio encargada de la lógica relacionada con las categorías del sistema.
/// </summary>
public class CategoryBusiness
{
    private readonly CategoryData _categoryData;
    private readonly ILogger <CategoryBusiness> _logger;

    public CategoryBusiness(CategoryData categoryData, ILogger <CategoryBusiness> logger)
    {
        _categoryData = categoryData;
        _logger = logger;
    }

    // Método para obtener todas las categorías como DTOs
    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        try
        {
            var categories = await _categoryData.GetAllAsync();
            return MapToDTOList(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las categorías");
            throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de categorías", ex);
        }
    }

    // Método para obtener una categoría por ID como DTO
    public async Task<CategoryDto> GetCategoryByIdAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó obtener una categoría con ID inválido: {CategoryId}", id);
            throw new Utilities.Exceptions.ValidationException("id", "El ID de la categoría debe ser mayor que cero");
        }

        try
        {
            var category = await _categoryData.GetByIdAsync(id);
            if (category == null)
            {
                _logger.LogInformation("No se encontró ninguna categoría con ID: {CategoryId}", id);
                throw new EntityNotFoundException("Category", id);
            }

            return MapToDTO(category);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la categoría con ID: {CategoryId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al recuperar la categoría con ID {id}", ex);
        }
    }

    // Método para crear una categoría desde un DTO
    public async Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto)
    {
        try
        {
            ValidateCategory(categoryDto);

            var category = MapToEntity(categoryDto);

            var categoryCreated = await _categoryData.CreateAsync(category);

            return MapToDTO(categoryCreated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear nueva categoría: {CategoryName}", categoryDto?.NameCategory ?? "null");
            throw new ExternalServiceException("Base de datos", "Error al crear la categoría", ex);
        }
    }

    // Método para actualizar una categoría completa
    public async Task<bool> UpdateCategoryAsync(CategoryDto categoryDto)
    {
        try
        {
            ValidateCategory(categoryDto);

            var category = MapToEntity(categoryDto);

            var result = await _categoryData.UpdateAsync(category);

            if (!result)
            {
                _logger.LogWarning("No se pudo actualizar la categoría con ID {CategoryId}", categoryDto.Id);
                throw new EntityNotFoundException("Category", categoryDto.Id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la categoría con ID {CategoryId}", categoryDto.Id);
            throw new ExternalServiceException("Base de datos", $"Error al actualizar la categoría con ID {categoryDto.Id}", ex);
        }
    }

    // Método para actualización parcial de una categoría
    public async Task<bool> UpdatePartialCategoryAsync(int id, CategoryDto updatedFields)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó actualizar una categoría con un ID inválido: {CategoryId}", id);
            throw new ValidationException("id", "El ID de la categoría debe ser mayor a 0");
        }

        try
        {
            var existingCategory = await _categoryData.GetByIdAsync(id);
            if (existingCategory == null)
            {
                _logger.LogInformation("No se encontró la categoría con ID {CategoryId} para actualización parcial", id);
                throw new EntityNotFoundException("Category", id);
            }

            // Solo actualizar campos que estén definidos
            if (!string.IsNullOrWhiteSpace(updatedFields.NameCategory))
            {
                existingCategory.NameCategory = updatedFields.NameCategory;
            }

            if (!string.IsNullOrWhiteSpace(updatedFields.Description))
            {
                existingCategory.Description = updatedFields.Description;
            }

            var result = await _categoryData.UpdateAsync(existingCategory);

            if (!result)
            {
                _logger.LogWarning("No se pudo actualizar parcialmente la categoría con ID {CategoryId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente la categoría con ID {id}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar parcialmente la categoría con ID {CategoryId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente la categoría con ID {id}", ex);
        }
    }

    // Método para realizar un Soft Delete (eliminación lógica)
    public async Task<bool> SoftDeleteCategoryAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó realizar una eliminación lógica con un ID inválido: {CategoryId}", id);
            throw new ValidationException("id", "El ID de la categoría debe ser mayor a 0");
        }

        try
        {
            var category = await _categoryData.GetByIdAsync(id);
            if (category == null)
            {
                _logger.LogInformation("No se encontró la categoría con ID {CategoryId} para eliminación lógica", id);
                throw new EntityNotFoundException("Category", id);
            }

            //category.Status = false; // Asegúrate de que el modelo Category tenga un campo Status

            var result = await _categoryData.UpdateAsync(category);

            if (!result)
            {
                _logger.LogWarning("No se pudo realizar la eliminación lógica de la categoría con ID {CategoryId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica de la categoría con ID {id}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al realizar la eliminación lógica de la categoría con ID {CategoryId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica de la categoría con ID {id}", ex);
        }
    }

    // Método para eliminación física de una categoría
    public async Task<bool> DeleteCategoryAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó eliminar una categoría con un ID inválido: {CategoryId}", id);
            throw new ValidationException("id", "El ID de la categoría debe ser mayor a 0");
        }

        try
        {
            var result = await _categoryData.DeleteAsync(id);

            if (!result)
            {
                _logger.LogInformation("No se encontró la categoría con ID {CategoryId} para eliminar", id);
                throw new EntityNotFoundException("Category", id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la categoría con ID {CategoryId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al eliminar la categoría con ID {id}", ex);
        }
    }



    // Método para validar el DTO
    private void ValidateCategory(CategoryDto categoryDto)
    {
        if (categoryDto == null)
        {
            throw new Utilities.Exceptions.ValidationException("El objeto categoría no puede ser nulo");
        }

        if (string.IsNullOrWhiteSpace(categoryDto.NameCategory))
        {
            _logger.LogWarning("Se intentó crear/actualizar una categoría con Name vacío");
            throw new Utilities.Exceptions.ValidationException("Name", "El Name de la categoría es obligatorio");
        }
    }

    // Método para mapear de category a CategoryDto
    private CategoryDto MapToDTO(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            NameCategory = category.NameCategory,
            Description = category.Description,
        };
    }

    // Método para mapear de CategoryDto a category
    private Category MapToEntity(CategoryDto categoryDto)
    {
        return new Category
        {
            Id = categoryDto.Id,
            NameCategory = categoryDto.NameCategory,
            Description = categoryDto.Description,
        };
    }

    // Método para mapear una lista de Category a una lista de CategoryDto
    private IEnumerable<CategoryDto> MapToDTOList(IEnumerable<Category> categories)
    {
        var CategoryDto = new List<CategoryDto>();
        foreach (var category in categories)
        {
            CategoryDto.Add(MapToDTO(category));
        }
        return CategoryDto;
    }
}
