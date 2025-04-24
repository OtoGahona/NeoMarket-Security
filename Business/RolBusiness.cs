

using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business;

/// <summary>
/// Clase de negocio encargada de la lógica relacionada con los roles del sistema.
/// </summary>
public class RolBusiness
{
    private readonly RolData _rolData;
    private readonly ILogger <RolBusiness> _logger;

    public RolBusiness(RolData rolData, ILogger <RolBusiness> logger)
    {
        _rolData = rolData;
        _logger = logger;
    }

    // Método para obtener todos los roles como DTOs
    public async Task<IEnumerable<RolDto>> GetAllRolesAsync()
    {
        try
        {
            var roles = await _rolData.GetAllAsync();
            return MapToDTOList(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los roles");
            throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de roles", ex);
        }
    }

    // Método para obtener un rol por ID como DTO
    public async Task<RolDto> GetRolByIdAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó obtener un rol con ID inválido: {RolId}", id);
            throw new Utilities.Exceptions.ValidationException("id", "El ID del rol debe ser mayor que cero");
        }

        try
        {
            var rol = await _rolData.GetByidAsync(id);
            if (rol == null)
            {
                _logger.LogInformation("No se encontró ningún rol con ID: {RolId}", id);
                throw new EntityNotFoundException("Rol", id);
            }

            return MapToDTO(rol);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el rol con ID: {RolId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al recuperar el rol con ID {id}", ex);
        }
    }

    // Método para crear un rol desde un DTO
    public async Task<RolDto> CreateRolAsync(RolDto RolDto)
    {
        try
        {
            ValidateRol(RolDto);

            var rol = MapToEntity(RolDto);

            var rolCreado = await _rolData.CreateAsync(rol);
            return MapToDTO(rolCreado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear nuevo rol: {RolNombre}", RolDto?.NameRol ?? "null");
            throw new ExternalServiceException("Base de datos", "Error al crear el rol", ex);
        }
    }

    // Método para actualizar un rol existente
    public async Task<bool> UpdateRolAsync(RolDto rolDto)
    {
        try
        {
            ValidateRol(rolDto);

            var rol = MapToEntity(rolDto);

            var result = await _rolData.UpdateAsync(rol);

            if (!result)
            {
                _logger.LogWarning("No se pudo actualizar el rol con ID {RolId}", rolDto.Id);
                throw new EntityNotFoundException("Rol", rolDto.Id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el rol con ID {RolId}", rolDto.Id);
            throw new ExternalServiceException("Base de datos", $"Error al actualizar el rol con ID {rolDto.Id}", ex);
        }
    }

    // Método para actualizar campos específicos de un rol
    public async Task<bool> UpdatePartialRolAsync(int id, RolDto updatedFields)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó actualizar un rol con un ID inválido: {RolId}", id);
            throw new ValidationException("id", "El ID del rol debe ser mayor a 0");
        }

        try
        {
            // Obtener el rol existente
            var existingRol = await _rolData.GetByidAsync(id);
            if (existingRol == null)
            {
                _logger.LogInformation("No se encontró el rol con ID {RolId} para actualización parcial", id);
                throw new EntityNotFoundException("Rol", id);
            }

            // Actualizar solo los campos proporcionados en el DTO
            if (!string.IsNullOrWhiteSpace(updatedFields.NameRol))
            {
                existingRol.NameRol = updatedFields.NameRol;
            }
            if (!string.IsNullOrWhiteSpace(updatedFields.Description))
            {
                existingRol.Description = updatedFields.Description;
            }
            if (updatedFields.Status != existingRol.Status) // Si Active es un booleano, siempre se envía un valor
            {
                existingRol.Status = updatedFields.Status;
            }

            // Guardar los cambios
            var result = await _rolData.UpdateAsync(existingRol);

            if (!result)
            {
                _logger.LogWarning("No se pudo actualizar parcialmente el rol con ID {RolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el rol con ID {id}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar parcialmente el rol con ID {RolId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el rol con ID {id}", ex);
        }
    }


    // Método para realizar una eliminación lógica de un rol (marcar como inactivo)
    public async Task<bool> SoftDeleteRolAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó realizar una eliminación lógica con un ID inválido: {RolId}", id);
            throw new Utilities.Exceptions.ValidationException("id", "El ID del rol debe ser mayor a 0");
        }

        try
        {
            // Obtener el rol por ID
            var rol = await _rolData.GetByidAsync(id);
            if (rol == null)
            {
                _logger.LogInformation("No se encontró el rol con ID {RolId} para eliminación lógica", id);
                throw new EntityNotFoundException("Rol", id);
            }

            // Marcar el rol como inactivo
            rol.Status = false;

            // Actualizar el rol en la base de datos
            var result = await _rolData.UpdateAsync(rol);

            if (!result)
            {
                _logger.LogWarning("No se pudo realizar la eliminación lógica del rol con ID {RolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica del rol con ID {id}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al realizar la eliminación lógica del rol con ID {RolId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica del rol con ID {id}", ex);
        }
    }

    // Método para eliminar un rol por su ID
    public async Task<bool> DeleteRolAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó eliminar un rol con un ID inválido: {RolId}", id);
            throw new Utilities.Exceptions.ValidationException("id", "El ID del rol debe ser mayor a 0");
        }

        try
        {
            var result = await _rolData.DeleteAsync(id);

            if (!result)
            {
                _logger.LogInformation("No se encontró el rol con ID {RolId} para eliminar", id);
                throw new EntityNotFoundException("Rol", id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el rol con ID {RolId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al eliminar el rol con ID {id}", ex);
        }
    }

    // Método para validar el DTO
    private void ValidateRol(RolDto RolDto)
    {
        if (RolDto == null)
        {
            throw new Utilities.Exceptions.ValidationException("El objeto rol no puede ser nulo");
        }

        if (string.IsNullOrWhiteSpace(RolDto.NameRol))
        {
            _logger.LogWarning("Se intentó crear/actualizar un rol con Name vacío");
            throw new Utilities.Exceptions.ValidationException("Name", "El Name del rol es obligatorio");
        }
    }

    // Método para mapear de Rol a RolDTO
    private RolDto MapToDTO(Rol rol)
    {
        return new RolDto
        {
            Id = rol.Id,
            NameRol = rol.NameRol,
            Description = rol.Description,
            Status = rol.Status,
            IdUser = rol.IdUser
        };
    }

    // Método para mapear de RolDTO a Rol
    private Rol MapToEntity(RolDto rolDTO)
    {
        return new Rol
        {
            Id = rolDTO.Id,
            NameRol = rolDTO.NameRol,
            Description = rolDTO.Description,
            Status = rolDTO.Status,
        };
    }

    // Método para mapear una lista de Rol a una lista de RolDTO
    private IEnumerable<RolDto> MapToDTOList(IEnumerable<Rol> roles)
    {
        var rolesDTO = new List<RolDto>();
        foreach (var rol in roles)
        {
            rolesDTO.Add(MapToDTO(rol));
        }
        return rolesDTO;
    }
}