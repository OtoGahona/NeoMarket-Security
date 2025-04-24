using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business;

/// <summary>
/// Clase de negocio encargada de la lógica relacionada con el inventario del sistema.
/// </summary>
public class InventoryBusiness
{
    private readonly InventoryData _inventoryData;
    private readonly ILogger<InventoryBusiness> _logger;

    public InventoryBusiness(InventoryData inventoryData, ILogger<InventoryBusiness> logger)
    {
        _inventoryData = inventoryData;
        _logger = logger;
    }

    // Método para obtener todos los inventarios como DTOs
    public async Task<IEnumerable<InventoryDto>> GetAllInventoriesAsync()
    {
        try
        {
            var inventories = await _inventoryData.GetAllAsync();
            return MapToDTOList(inventories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los inventarios");
            throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de inventarios", ex);
        }
    }

    // Método para obtener un inventario por ID como DTO
    public async Task<InventoryDto> GetInventoryByIdAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó obtener un inventario con ID inválido: {InventoryId}", id);
            throw new Utilities.Exceptions.ValidationException("id", "El ID del inventario debe ser mayor que cero");
        }

        try
        {
            var inventory = await _inventoryData.GetByIdAsync(id);
            if (inventory == null)
            {
                _logger.LogInformation("No se encontró ningún inventario con ID: {InventoryId}", id);
                throw new EntityNotFoundException("Inventory", id);
            }
            return MapToDTO(inventory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el inventario con ID: {InventoryId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al recuperar el inventario con ID {id}", ex);
        }
    }

    // Método para crear un inventario desde un DTO
    public async Task<InventoryDto> CreateInventoryAsync(InventoryDto inventoryDto)
    {
        try
        {
            ValidateInventory(inventoryDto);

            var inventory = MapToEntity(inventoryDto);
            var createdInventory = await _inventoryData.CreateAsync(inventory);

            return MapToDTO(createdInventory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear nuevo inventario: {ProductName}", inventoryDto?.NameInventory ?? "null");
            throw new ExternalServiceException("Base de datos", "Error al crear el inventario", ex);
        }
    }

    // Método para actualizar un inventario existente
    public async Task<InventoryDto> UpdateInventoryAsync(int id, InventoryDto updatedFields)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó actualizar un inventario con ID inválido: {InventoryId}", id);
            throw new ValidationException("id", "El ID del inventario debe ser mayor que cero");
        }

        try
        {
            var existingInventory = await _inventoryData.GetByIdAsync(id);
            if (existingInventory == null)
            {
                _logger.LogInformation("No se encontró ningún inventario con ID: {InventoryId}", id);
                throw new EntityNotFoundException("Inventory", id);
            }

            // Actualizar solo los campos proporcionados
            if (!string.IsNullOrWhiteSpace(updatedFields.NameInventory))
                existingInventory.NameInventory = updatedFields.NameInventory;

            existingInventory.Status = updatedFields.Status;
            existingInventory.DescriptionInvetory = updatedFields.DescriptionInvetory;
            existingInventory.Observations = updatedFields.Observations;
            existingInventory.ZoneProduct = updatedFields.ZoneProduct;

            // Ejecutar la actualización
            var success = await _inventoryData.UpdateAsync(existingInventory);
            if (!success)
            {
                throw new ExternalServiceException("Base de datos", "No se pudo actualizar el inventario");
            }

            return MapToDTO(existingInventory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el inventario con ID: {InventoryId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al actualizar el inventario con ID {id}", ex);
        }
    }

    // Método para eliminar un inventario de forma lógica
    public async Task<bool> SoftDeleteInventoryAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó eliminar un inventario con ID inválido: {InventoryId}", id);
            throw new ValidationException("id", "El ID del inventario debe ser mayor que cero");
        }

        try
        {
            var inventory = await _inventoryData.GetByIdAsync(id);
            if (inventory == null)
            {
                _logger.LogInformation("No se encontró ningún inventario con ID: {InventoryId}", id);
                throw new EntityNotFoundException("Inventory", id);
            }

            // Marcar el inventario como eliminado (o inactivo)
            inventory.Status = false; 

            var success = await _inventoryData.UpdateAsync(inventory);
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el inventario con ID: {InventoryId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al eliminar el inventario con ID {id}", ex);
        }
    }

    // Método para eliminar un inventario de forma definitiva
    public async Task<bool> DeleteInventoryAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó eliminar un inventario con ID inválido: {InventoryId}", id);
            throw new ValidationException("id", "El ID del inventario debe ser mayor que cero");
        }

        try
        {
            // Verificar si el inventario existe en la base de datos
            var inventory = await _inventoryData.GetByIdAsync(id);
            if (inventory == null)
            {
                _logger.LogInformation("No se encontró ningún inventario con ID: {InventoryId}", id);
                throw new EntityNotFoundException("Inventory", id);
            }

            // Si el inventario se encuentra, proceder con la eliminación
            var success = await _inventoryData.DeleteAsync(inventory);

            if (!success)
            {
                _logger.LogWarning("No se pudo eliminar el inventario con ID: {InventoryId}", id);
                throw new ExternalServiceException("Base de datos", $"No se pudo eliminar el inventario con ID {id}");
            }

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el inventario con ID: {InventoryId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al eliminar el inventario con ID {id}", ex);
        }

    }

    // Método para validar el DTO
    private void ValidateInventory(InventoryDto inventoryDto)
    {
        if (inventoryDto == null)
        {
            throw new Utilities.Exceptions.ValidationException("El objeto inventario no puede ser nulo");
        }

        if (string.IsNullOrWhiteSpace(inventoryDto.NameInventory))
        {
            _logger.LogWarning("Se intentó crear/actualizar un inventario con ProductName vacío");
            throw new Utilities.Exceptions.ValidationException("ProductName", "El nombre del producto es obligatorio");
        }
    }

    // Método para mapear de Inventory a InventoryDto
    private InventoryDto MapToDTO(Inventory inventory)
    {
        return new InventoryDto
        {
            Id = inventory.Id,
            NameInventory = inventory.NameInventory,
            Status = inventory.Status,
            DescriptionInvetory = inventory.DescriptionInvetory,
            Observations = inventory.Observations,
            ZoneProduct = inventory.ZoneProduct
        };
    }

    // Método para mapear de InventoryDto a Inventory
    private Inventory MapToEntity(InventoryDto inventoryDto)
    {
        return new Inventory
        {
            Id = inventoryDto.Id,
            NameInventory = inventoryDto.NameInventory,
            Status = inventoryDto.Status,
            DescriptionInvetory = inventoryDto.DescriptionInvetory,
            Observations = inventoryDto.Observations,
            ZoneProduct = inventoryDto.ZoneProduct
        };
    }

    // Método para mapear una lista de Inventory a una lista de InventoryDto
    private IEnumerable<InventoryDto> MapToDTOList(IEnumerable<Inventory> inventories)
    {
        var inventoriesDto = new List<InventoryDto>();
        foreach (var inventory in inventories)
        {
            inventoriesDto.Add(MapToDTO(inventory));
        }
        return inventoriesDto;
    }

    public async Task UpdateAsync(InventoryDto inventoryDto)
    {
        throw new NotImplementedException();
    }
}
