using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business;

/// <summary>
/// Clase de negocio encargada de la lógica relacionada con los módulos del sistema.
/// </summary>
public class ModuleBusiness
{
    private readonly ModuleData _moduleData;
    private readonly ILogger <ModuleBusiness> _logger;

    public ModuleBusiness(ModuleData moduleData, ILogger<ModuleBusiness> logger)
    {
        _moduleData = moduleData;
        _logger = logger;
    }

    // Método para obtener todos los módulos como DTOs
    public async Task<IEnumerable<ModuleDto>> GetAllModulesAsync()
    {
        try
        {
            var modules = await _moduleData.GetAllAsync();
            return MapToDTOList(modules);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los módulos");
            throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de módulos", ex);
        }
    }

    // Método para obtener un módulo por ID como DTO
    public async Task<ModuleDto> GetModuleByIdAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó obtener un módulo con ID inválido: {ModuleId}", id);
            throw new ValidationException("id", "El ID del módulo debe ser mayor que cero");
        }

        try
        {
            var module = await _moduleData.GetByIdAsync(id);
            if (module == null)
            {
                _logger.LogInformation("No se encontró ningún módulo con ID: {ModuleId}", id);
                throw new EntityNotFoundException("Module", id);
            }

            return MapToDTO(module);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el módulo con ID: {ModuleId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al recuperar el módulo con ID {id}", ex);
        }
    }

    // Método para crear un módulo desde un DTO
    public async Task<ModuleDto> CreateModuleAsync(ModuleDto moduleDto)
    {
        try
        {
            ValidateModule(moduleDto);

            var module = MapToEntity(moduleDto);

            var moduleCreado = await _moduleData.CreateAsync(module);

            return MapToDTO(moduleCreado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear nuevo módulo: {ModuleNombre}", moduleDto?.NameModule ?? "null");
            throw new ExternalServiceException("Base de datos", "Error al crear el módulo", ex);
        }
    }

    public async Task<bool> UpdateModuleAsync(ModuleDto moduleDto)
    {
        try
        {
            ValidateModule(moduleDto);

            var module = MapToEntity(moduleDto);

            var result = await _moduleData.UpdateAsync(module);

            if (!result)
            {
                _logger.LogWarning("No se pudo actualizar el módulo con ID {ModuleId}", moduleDto.Id);
                throw new EntityNotFoundException("Module", moduleDto.Id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el módulo con ID {ModuleId}", moduleDto.Id);
            throw new ExternalServiceException("Base de datos", $"Error al actualizar el módulo con ID {moduleDto.Id}", ex);
        }
    }

    public async Task<bool> UpdatePartialModuleAsync(int id, ModuleDto updatedFields)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó actualizar un módulo con un ID inválido: {ModuleId}", id);
            throw new ValidationException("id", "El ID del módulo debe ser mayor a 0");
        }

        try
        {
            var existingModule = await _moduleData.GetByIdAsync(id);
            if (existingModule == null)
            {
                _logger.LogInformation("No se encontró el módulo con ID {ModuleId} para actualización parcial", id);
                throw new EntityNotFoundException("Module", id);
            }

            if (!string.IsNullOrWhiteSpace(updatedFields.NameModule))
            {
                existingModule.NameModule = updatedFields.NameModule;
            }
            if (updatedFields.status != existingModule.status)
            {
                existingModule.status = updatedFields.status;
            }

            var result = await _moduleData.UpdateAsync(existingModule);

            if (!result)
            {
                _logger.LogWarning("No se pudo actualizar parcialmente el módulo con ID {ModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el módulo con ID {id}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar parcialmente el módulo con ID {ModuleId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el módulo con ID {id}", ex);
        }
    }

    public async Task<bool> SoftDeleteModuleAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó realizar una eliminación lógica con un ID inválido: {ModuleId}", id);
            throw new ValidationException("id", "El ID del módulo debe ser mayor a 0");
        }

        try
        {
            var module = await _moduleData.GetByIdAsync(id);
            if (module == null)
            {
                _logger.LogInformation("No se encontró el módulo con ID {ModuleId} para eliminación lógica", id);
                throw new EntityNotFoundException("Module", id);
            }

            module.status = true;

            var result = await _moduleData.UpdateAsync(module);

            if (!result)
            {
                _logger.LogWarning("No se pudo realizar la eliminación lógica del módulo con ID {ModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica del módulo con ID {id}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al realizar la eliminación lógica del módulo con ID {ModuleId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica del módulo con ID {id}", ex);
        }
    }

    public async Task<bool> DeleteModuleAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó eliminar un módulo con un ID inválido: {ModuleId}", id);
            throw new ValidationException("id", "El ID del módulo debe ser mayor a 0");
        }

        try
        {
            var result = await _moduleData.DeleteAsync(id);

            if (!result)
            {
                _logger.LogInformation("No se encontró el módulo con ID {ModuleId} para eliminar", id);
                throw new EntityNotFoundException("Module", id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el módulo con ID {ModuleId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al eliminar el módulo con ID {id}", ex);
        }
    }


    // Método para validar el DTO
    private void ValidateModule(ModuleDto moduleDto)
    {
        if (moduleDto == null)
        {
            throw new ValidationException("El objeto módulo no puede ser nulo");
        }

        if (string.IsNullOrWhiteSpace(moduleDto.NameModule))
        {
            _logger.LogWarning("Se intentó crear/actualizar un módulo con Name vacío");
            throw new ValidationException("Name", "El Name del módulo es obligatorio");
        }
    }

    // Método para mapear de Module a ModuleDto
    private ModuleDto MapToDTO(Module module)
    {
        return new ModuleDto
        {
            Id = module.Id,
            NameModule = module.NameModule,
            status = module.status
        };
    }

    // Método para mapear de ModuleDto a Module
    private Module MapToEntity(ModuleDto moduleDto)
    {
        return new Module
        {
            Id = moduleDto.Id,
            NameModule = moduleDto.NameModule,
            status = moduleDto.status
        };
    }

    // Método para mapear una lista de Module a una lista de ModuleDto
    private IEnumerable<ModuleDto> MapToDTOList(IEnumerable<Module> modules)
    {
        var modulesDTO = new List<ModuleDto>();
        foreach (var module in modules)
        {
            modulesDTO.Add(MapToDTO(module));
        }
        return modulesDTO;
    }
}

