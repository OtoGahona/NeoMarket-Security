using Data;
using Entity.DTO;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Server;
using Utilities.Exceptions;

namespace Business;

/// <summary>
/// Clase de negocio encargada de la lógica relacionada con los formularios del sistema.
/// </summary>
public class FormBusiness
{
    private readonly FormData _formData;
    private readonly ILogger <FormBusiness> _logger;

    public FormBusiness(FormData formData, ILogger <FormBusiness> logger)
    {
        _formData = formData;
        _logger = logger;
    }

    // Método para obtener todos los formularios como DTOs
    public async Task<IEnumerable<FormDto>> GetAllFormsAsync()
    {
        try
        {
            var forms = await _formData.GetAllAsync();
            return MapToDTOList(forms);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los formularios");
            throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de formularios", ex);
        }
    }

    // Método para obtener un formulario por ID como DTO
    public async Task<FormDto> GetFormByIdAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó obtener un formulario con ID inválido: {FormId}", id);
            throw new Utilities.Exceptions.ValidationException("id", "El ID del formulario debe ser mayor que cero");
        }

        try
        {
            var form = await _formData.GetByIdAsync(id);
            if (form == null)
            {
                _logger.LogInformation("No se encontró ningún formulario con ID: {FormId}", id);
                throw new EntityNotFoundException("Form", id);
            }

            return MapToDTO(form);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el formulario con ID: {FormId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al recuperar el formulario con ID {id}", ex);
        }
    }

    // Método para crear un formulario desde un DTO
    public async Task<FormDto> CreateFormAsync(FormDto formDto)
    {
        try
        {
            ValidateForm(formDto);

            var form = MapToEntity(formDto);

            var createdForm = await _formData.CreateAsync(form);

            return MapToDTO(createdForm);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear nuevo formulario: {FormTitle}", formDto?.NameForm ?? "null");
            throw new ExternalServiceException("Base de datos", "Error al crear el formulario", ex);
        }
    }

    // Método para actualizar parcialmente un formulario (UpdatePartial)
    public async Task<FormDto> UpdatePartialAsync(int id, FormDto updatedFields)
    {
        if (id <= 0)
        {
            throw new Utilities.Exceptions.ValidationException("id", "El ID del formulario debe ser mayor que cero");
        }

        try
        {
            var existingForm = await _formData.GetByIdAsync(id);
            if (existingForm == null)
            {
                throw new EntityNotFoundException("Form", id);
            }

            if (!string.IsNullOrWhiteSpace(updatedFields.NameForm))
                existingForm.NameForm = updatedFields.NameForm;

            if (!string.IsNullOrWhiteSpace(updatedFields.Description))
                existingForm.Description = updatedFields.Description;

                existingForm.Status = updatedFields.Status;

            await _formData.UpdateAsync(existingForm);

            return MapToDTO(existingForm);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar parcialmente el formulario con ID: {FormId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al actualizar el formulario con ID {id}", ex);
        }
    }

    // Método para actualizar una compra existente

    public async Task<bool> UpdateFormAsync(FormDto formDto)
    {
        try
        {
            // Validamos los datos recibidos del formulario
            ValidateForm(formDto);

            // Mapeamos el DTO a la entidad correspondiente
            var formEntity = MapToEntity(formDto);

            // Realizamos la actualización del formulario en la base de datos
            var result = await _formData.UpdateAsync(formEntity);

            // Verificamos si la actualización fue exitosa
            if (!result)
            {
                _logger.LogWarning("No se pudo actualizar el formulario con ID {FormId}", formDto.Id);
                throw new EntityNotFoundException("Formulario", formDto.Id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el formulario con ID {FormId}", formDto.Id);
            throw new ExternalServiceException("Base de datos", $"Error al actualizar el formulario con ID {formDto.Id}", ex);
        }
    }

    // Método para eliminar lógicamente un formulario (SoftDelete)
    public async Task<bool> SoftDeleteAsync(int id)
    {
        if (id <= 0)
        {
            throw new Utilities.Exceptions.ValidationException("id", "El ID del formulario debe ser mayor que cero");
        }

        try
        {
            var existingForm = await _formData.GetByIdAsync(id);
            if (existingForm == null)
            {
                throw new EntityNotFoundException("Form", id);
            }

            existingForm.Status = false;
            await _formData.UpdateAsync(existingForm);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar lógicamente el formulario con ID: {FormId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al eliminar el formulario con ID {id}", ex);
        }
    }


    // Método para validar el DTO
    private void ValidateForm(FormDto formDto)
    {
        if (formDto == null)
        {
            throw new Utilities.Exceptions.ValidationException("El objeto formulario no puede ser nulo");
        }

        if (string.IsNullOrWhiteSpace((string?)formDto.NameForm))
        {
            _logger.LogWarning("Se intentó crear/actualizar un formulario con Title vacío");
            throw new Utilities.Exceptions.ValidationException("Title", "El título del formulario es obligatorio");
        }
    }

    // Método para mapear de Form a FormDto
    private FormDto MapToDTO(Form form)
    {
        return new FormDto
        {
            Id = form.Id,
            Description = form.Description,
            Status = form.Status,
            NameForm = form.NameForm,
            IdModule = form.IdModule
        };
    }

    // Método para mapear de FormDto a Form
    private Form MapToEntity(FormDto formDto)
    {
        return new Form
        {
            Id = formDto.Id,
            NameForm = formDto.NameForm,
            Description = formDto.Description,
            Status = formDto.Status,
            IdModule = formDto.IdModule
        };
    }

    // Método para mapear una lista de Rol a una lista de RolDTO
    private IEnumerable<FormDto> MapToDTOList(IEnumerable<Form> forms)
    {
        var formDto = new List<FormDto>();
        foreach (var form in forms)
        {
            formDto.Add(MapToDTO(form));
        }
        return formDto;
    }

    public async Task UpdateAsync(FormDto formDto)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteFormAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task SoftDeleteFormAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task UpdatePartialFormAsync(int id, FormDto updatedFields)
    {
        throw new NotImplementedException();
    }

    public async Task UpdatePartialAsync(int id, Dictionary<string, object> fields)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(int id, FormDto formDto)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteformAsync(int id)
    {
        throw new NotImplementedException();
    }
}