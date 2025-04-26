using Data;
using Entity.DTO;
using Entity.Enum;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business;

/// <summary>
/// Clase de negocio encargada de la lógica relacionada con los formularios de roles en el sistema.
/// </summary>
public class RolFormBusiness
{
    private readonly RolFormData _rolFormData;
    private readonly ILogger <RolFormBusiness> _logger;

    public RolFormBusiness(RolFormData rolFormData, ILogger <RolFormBusiness> logger)
    {
        _rolFormData = rolFormData;
        _logger = logger;
    }

    // Método para obtener todos los formularios de roles como DTOs
    public async Task<IEnumerable<RolFormDto>> GetAllRolFormsAsync()
    {
        try
        {
            var rolForms = await _rolFormData.GetAllAsync();
            return MapToDTOList(rolForms);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los formularios de roles");
            throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de formularios de roles", ex);
        }
    }

    // Método para obtener un formulario de rol por ID como DTO
    public async Task<RolFormDto> GetRolFormByIdAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó obtener un formulario de rol con ID inválido: {RolFormId}", id);
            throw new Utilities.Exceptions.ValidationException("id", "El ID del formulario de rol debe ser mayor que cero");
        }

        try
        {
            var rolForm = await _rolFormData.GetByIdAsync(id);
            if (rolForm == null)
            {
                _logger.LogInformation("No se encontró ningún formulario de rol con ID: {RolFormId}", id);
                throw new EntityNotFoundException("Formulario de rol", id);
            }

            return MapToDTO(rolForm);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el formulario de rol con ID: {RolFormId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al recuperar el formulario de rol con ID {id}", ex);
        }
    }

    // Método para crear un formulario de rol desde un DTO
    public async Task<RolFormDto> CreateRolFormAsync(RolFormDto rolFormDto)
    {
        try
        {
            ValidateRolForm(rolFormDto);

            var rolForm = MapToEntity(rolFormDto);

            var rolFormCreado = await _rolFormData.CreateAsync(rolForm);
            return MapToDTO(rolFormCreado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear nuevo formulario de rol: {RolFormNombre}", rolFormDto?.Id);
            throw new ExternalServiceException("Base de datos", "Error al crear el formulario de rol", ex);
        }
    }

    /// <summary>
    /// Actualiza un formulario de rol existente en la base de datos.
    /// </summary>
    /// <param name="rolFormDto">El DTO del formulario de rol con los datos actualizados.</param>
    /// <returns>True si la actualización fue exitosa, False en caso contrario.</returns>
    public async Task<bool> UpdateRolFormAsync(RolFormDto rolFormDto)
    {
        try
        {
            ValidateRolForm(rolFormDto);

            var rolForm = MapToEntity(rolFormDto);

            var result = await _rolFormData.UpdateAsync(rolForm);

            if (!result)
            {
                _logger.LogWarning("No se pudo actualizar el formulario de rol con ID {RolFormId}", rolFormDto.Id);
                throw new EntityNotFoundException("Formulario de rol", rolFormDto.Id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el formulario de rol con ID {RolFormId}", rolFormDto.Id);
            throw new ExternalServiceException("Base de datos", $"Error al actualizar el formulario de rol con ID {rolFormDto.Id}", ex);
        }
    }

    /// <summary>
    /// Actualiza campos específicos de un formulario de rol existente en la base de datos.
    /// </summary>
    /// <param name="id">El ID del formulario de rol a actualizar.</param>
    /// <param name="updatedFields">El DTO con los campos actualizados.</param>
    /// <returns>True si la actualización fue exitosa, False en caso contrario.</returns>
    public async Task<bool> UpdatePartialRolFormAsync(int id, RolFormDto updatedFields)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó actualizar un formulario de rol con un ID inválido: {RolFormId}", id);
            throw new ValidationException("id", "El ID del formulario de rol debe ser mayor a 0");
        }

        try
        {
            var existingRolForm = await _rolFormData.GetByIdAsync(id);
            if (existingRolForm == null)
            {
                _logger.LogInformation("No se encontró el formulario de rol con ID {RolFormId} para actualización parcial", id);
                throw new EntityNotFoundException("Formulario de rol", id);
            }

            if (updatedFields.Permision != existingRolForm.Permision)
            {
                existingRolForm.Permision = updatedFields.Permision;
            }

            var result = await _rolFormData.UpdateAsync(existingRolForm);

            if (!result)
            {
                _logger.LogWarning("No se pudo actualizar parcialmente el formulario de rol con ID {RolFormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el formulario de rol con ID {id}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar parcialmente el formulario de rol con ID {RolFormId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el formulario de rol con ID {id}", ex);
        }
    }

    /// <summary>
    /// Realiza una eliminación lógica de un formulario de rol, marcándolo como inactivo.
    /// </summary>
    /// <param name="id">El ID del formulario de rol a eliminar lógicamente.</param>
    /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
    public async Task<bool> SoftDeleteRolFormAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó realizar una eliminación lógica con un ID inválido: {RolFormId}", id);
            throw new ValidationException("id", "El ID del formulario de rol debe ser mayor a 0");
        }

        try
        {
            var rolForm = await _rolFormData.GetByIdAsync(id);
            if (rolForm == null)
            {
                _logger.LogInformation("No se encontró el formulario de rol con ID {RolFormId} para eliminación lógica", id);
                throw new EntityNotFoundException("Formulario de rol", id);
            }

            rolForm.Permision = Permision.None; // Ejemplo: marcar como sin permisos

            var result = await _rolFormData.UpdateAsync(rolForm);

            if (!result)
            {
                _logger.LogWarning("No se pudo realizar la eliminación lógica del formulario de rol con ID {RolFormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica del formulario de rol con ID {id}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al realizar la eliminación lógica del formulario de rol con ID {RolFormId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica del formulario de rol con ID {id}", ex);
        }
    }

    /// <summary>
    /// Elimina un formulario de rol de la base de datos por su ID.
    /// </summary>
    /// <param name="id">El ID del formulario de rol a eliminar.</param>
    /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
    public async Task<bool> DeleteRolFormAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó eliminar un formulario de rol con un ID inválido: {RolFormId}", id);
            throw new ValidationException("id", "El ID del formulario de rol debe ser mayor a 0");
        }

        try
        {
            var result = await _rolFormData.DeleteAsync(id);

            if (!result)
            {
                _logger.LogInformation("No se encontró el formulario de rol con ID {RolFormId} para eliminar", id);
                throw new EntityNotFoundException("Formulario de rol", id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el formulario de rol con ID {RolFormId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al eliminar el formulario de rol con ID {id}", ex);
        }
    }


    // Método para validar el DTO
    private void ValidateRolForm(RolFormDto rolFormDto)
    {
        if (rolFormDto == null)
        {
            throw new Utilities.Exceptions.ValidationException("El objeto formulario de rol no puede ser nulo");
        }

        if (rolFormDto.Id < 0)
        {
            _logger.LogWarning("Se intentó crear/actualizar un formulario de rol con Name vacío");
            throw new Utilities.Exceptions.ValidationException("Name", "El Name del formulario de rol es obligatorio");
        }
    }

    // Método para mapear de RolForm a RolFormDTO
    private RolFormDto MapToDTO(RolForm rolForm)
    {
        return new RolFormDto
        {
            Id = rolForm.Id,
            Permision = rolForm.Permision,
            IdForm = rolForm.IdForm,
            IdRol = rolForm.IdRol
        };
    }

    // Método para mapear de RolFormDTO a RolForm
    private RolForm MapToEntity(RolFormDto rolFormDTO)
    {
        return new RolForm
        {
            Id = rolFormDTO.Id,
            Permision = rolFormDTO.Permision,
            IdRol = rolFormDTO.IdRol,
            IdForm =rolFormDTO.IdForm
        };
    }

    // Método para mapear una lista de RolForm a una lista de RolFormDTO
    private IEnumerable<RolFormDto> MapToDTOList(IEnumerable<RolForm> rolForms)
    {
        var rolFormsDTO = new List<RolFormDto>();
        foreach (var rolForm in rolForms)
        {
            rolFormsDTO.Add(MapToDTO(rolForm));
        }
        return rolFormsDTO;
    }
}
