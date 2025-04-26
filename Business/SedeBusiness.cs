using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business;

/// <summary>
/// Clase de negocio encargada de la lógica relacionada con las sedes del sistema.
/// </summary>
public class SedeBusiness
{
    private readonly SedeData _sedeData;
    private readonly ILogger <SedeBusiness> _logger;

    public SedeBusiness(SedeData sedeData, ILogger <SedeBusiness> logger)
    {
        _sedeData = sedeData;
        _logger = logger;
    }

    // Método para obtener todas las sedes como DTOs
    public async Task<IEnumerable<SedeDto>> GetAllSedesAsync()
    {
        try
        {
            var sedes = await _sedeData.GetAllAsync();
            return MapToDTOList(sedes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las sedes");
            throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de sedes", ex);
        }
    }

    // Método para obtener una sede por ID como DTO
    public async Task<SedeDto> GetSedeByIdAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó obtener una sede con ID inválido: {SedeId}", id);
            throw new ValidationException("id", "El ID de la sede debe ser mayor que cero");
        }

        try
        {
            var sede = await _sedeData.GetByIdAsync(id);
            if (sede == null)
            {
                _logger.LogInformation("No se encontró ninguna sede con ID: {SedeId}", id);
                throw new EntityNotFoundException("Sede", id);
            }

            return MapToDTO(sede);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la sede con ID: {SedeId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al recuperar la sede con ID {id}", ex);
        }
    }

    // Método para crear una sede desde un DTO
    public async Task<SedeDto> CreateSedeAsync(SedeDto sedeDto)
    {
        try
        {
            ValidateSede(sedeDto);

            var sede = MapToEntity(sedeDto);

            var sedeCreada = await _sedeData.CreateAsync(sede);

            return MapToDTO(sedeCreada);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear nueva sede: {SedeNombre}", sedeDto?.NameSede ?? "null");
            throw new ExternalServiceException("Base de datos", "Error al crear la sede", ex);
        }
    }

    /// <summary>
    /// Actualiza una sede existente en la base de datos.
    /// </summary>
    /// <param name="sedeDto">El DTO de la sede con los datos actualizados.</param>
    /// <returns>True si la actualización fue exitosa, False en caso contrario.</returns>
    public async Task<bool> UpdateSedeAsync(SedeDto sedeDto)
    {
        try
        {
            ValidateSede(sedeDto);

            var sede = MapToEntity(sedeDto);

            var result = await _sedeData.UpdateAsync(sede);

            if (!result)
            {
                _logger.LogWarning("No se pudo actualizar la sede con ID {SedeId}", sedeDto.Id);
                throw new EntityNotFoundException("Sede", sedeDto.Id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la sede con ID {SedeId}", sedeDto.Id);
            throw new ExternalServiceException("Base de datos", $"Error al actualizar la sede con ID {sedeDto.Id}", ex);
        }
    }

    /// <summary>
    /// Actualiza campos específicos de una sede existente en la base de datos.
    /// </summary>
    /// <param name="id">El ID de la sede a actualizar.</param>
    /// <param name="updatedFields">El DTO con los campos actualizados.</param>
    /// <returns>True si la actualización fue exitosa, False en caso contrario.</returns>
    public async Task<bool> UpdatePartialSedeAsync(int id, SedeDto updatedFields)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó actualizar una sede con un ID inválido: {SedeId}", id);
            throw new ValidationException("id", "El ID de la sede debe ser mayor a 0");
        }

        try
        {
            var existingSede = await _sedeData.GetByIdAsync(id);
            if (existingSede == null)
            {
                _logger.LogInformation("No se encontró la sede con ID {SedeId} para actualización parcial", id);
                throw new EntityNotFoundException("Sede", id);
            }

            if (!string.IsNullOrWhiteSpace(updatedFields.NameSede))
            {
                existingSede.NameSede = updatedFields.NameSede;
            }
            if (!string.IsNullOrWhiteSpace(updatedFields.CodeSede))
            {
                existingSede.CodeSede = updatedFields.CodeSede;
            }
            if (!string.IsNullOrWhiteSpace(updatedFields.AddressSede))
            {
                existingSede.AddressSede = updatedFields.AddressSede;
            }
            if (!string.IsNullOrWhiteSpace(updatedFields.EmailSede))
            {
                existingSede.EmailSede = updatedFields.EmailSede;
            }
            if (updatedFields.Status != existingSede.Status)
            {
                existingSede.Status = updatedFields.Status;
            }

            var result = await _sedeData.UpdateAsync(existingSede);

            if (!result)
            {
                _logger.LogWarning("No se pudo actualizar parcialmente la sede con ID {SedeId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente la sede con ID {id}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar parcialmente la sede con ID {SedeId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente la sede con ID {id}", ex);
        }
    }

    /// <summary>
    /// Realiza una eliminación lógica de una sede, marcándola como inactiva.
    /// </summary>
    /// <param name="id">El ID de la sede a eliminar lógicamente.</param>
    /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
    public async Task<bool> SoftDeleteSedeAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó realizar una eliminación lógica con un ID inválido: {SedeId}", id);
            throw new ValidationException("id", "El ID de la sede debe ser mayor a 0");
        }

        try
        {
            var sede = await _sedeData.GetByIdAsync(id);
            if (sede == null)
            {
                _logger.LogInformation("No se encontró la sede con ID {SedeId} para eliminación lógica", id);
                throw new EntityNotFoundException("Sede", id);
            }

            sede.Status = false; // Marcar como inactiva

            var result = await _sedeData.UpdateAsync(sede);

            if (!result)
            {
                _logger.LogWarning("No se pudo realizar la eliminación lógica de la sede con ID {SedeId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica de la sede con ID {id}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al realizar la eliminación lógica de la sede con ID {SedeId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica de la sede con ID {id}", ex);
        }
    }

    /// <summary>
    /// Elimina una sede de la base de datos por su ID.
    /// </summary>
    /// <param name="id">El ID de la sede a eliminar.</param>
    /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
    public async Task<bool> DeleteSedeAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó eliminar una sede con un ID inválido: {SedeId}", id);
            throw new ValidationException("id", "El ID de la sede debe ser mayor a 0");
        }

        try
        {
            var result = await _sedeData.DeleteAsync(id);

            if (!result)
            {
                _logger.LogInformation("No se encontró la sede con ID {SedeId} para eliminar", id);
                throw new EntityNotFoundException("Sede", id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la sede con ID {SedeId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al eliminar la sede con ID {id}", ex);
        }
    }

    // Método para validar el DTO
    private void ValidateSede(SedeDto sedeDto)
    {
        if (sedeDto == null)
        {
            throw new ValidationException("El objeto sede no puede ser nulo");
        }

        if (string.IsNullOrWhiteSpace(sedeDto.NameSede))
        {
            _logger.LogWarning("Se intentó crear/actualizar una sede con Name vacío");
            throw new ValidationException("Name", "El Name de la sede es obligatorio");
        }
    }

    // Método para mapear de Sede a SedeDto
    private SedeDto MapToDTO(Sede sede)
    {
        return new SedeDto
        {
            Id = sede.Id,
            NameSede = sede.NameSede,
            CodeSede = sede.CodeSede,
            AddressSede = sede.AddressSede,
            EmailSede = sede.EmailSede,
            Status = sede.Status,
            IdCompany = sede.IdCompany
        };
    }

    // Método para mapear de SedeDto a Sede
    private Sede MapToEntity(SedeDto sedeDto)
    {
        return new Sede
        {
            Id = sedeDto.Id,
            NameSede = sedeDto.NameSede,
            CodeSede = sedeDto.CodeSede,
            AddressSede = sedeDto.AddressSede,
            EmailSede = sedeDto.EmailSede,
            Status = sedeDto.Status,
            IdCompany =sedeDto.IdCompany
        };
    }

    // Método para mapear una lista de Sede a una lista de SedeDto
    private IEnumerable<SedeDto> MapToDTOList(IEnumerable<Sede> sedes)
    {
        var sedesDTO = new List<SedeDto>();
        foreach (var sede in sedes)
        {
            sedesDTO.Add(MapToDTO(sede));
        }
        return sedesDTO;
    }
}