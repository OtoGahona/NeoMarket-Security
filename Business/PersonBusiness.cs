using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business;

/// <summary>
/// Clase de negocio encargada de la lógica relacionada con las personas del sistema.
/// </summary>
public class PersonBusiness
{
    private readonly PersonData _personData;
    private readonly ILogger <PersonBusiness> _logger;

    public PersonBusiness(PersonData personData, ILogger<PersonBusiness> logger)
    {
        _personData = personData;
        _logger = logger;
    }

    // Método para obtener todas las personas como DTOs
    public async Task<IEnumerable<PersonDto>> GetAllPersonsAsync()
    {
        try
        {
            var persons = await _personData.GetAllAsync();
            return MapToDTOList(persons);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las personas");
            throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de personas", ex);
        }
    }

    // Método para obtener una persona por ID como DTO
    public async Task<PersonDto> GetPersonByIdAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó obtener una persona con ID inválido: {PersonId}", id);
            throw new ValidationException("id", "El ID de la persona debe ser mayor que cero");
        }

        try
        {
            var person = await _personData.GetByIdAsync(id);
            if (person == null)
            {
                _logger.LogInformation("No se encontró ninguna persona con ID: {PersonId}", id);
                throw new EntityNotFoundException("Person", id);
            }

            return MapToDTO(person);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la persona con ID: {PersonId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al recuperar la persona con ID {id}", ex);
        }
    }

    // Método para crear una persona desde un DTO
    public async Task<PersonDto> CreatePersonAsync(PersonDto personDto)
    {
        try
        {
            ValidatePerson(personDto);

            var person = MapToEntity(personDto);

            var personCreada = await _personData.CreateAsync(person);

            return MapToDTO(personCreada);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear nueva persona: {PersonNombre}", personDto?.FirstName ?? "null");
            throw new ExternalServiceException("Base de datos", "Error al crear la persona", ex);
        }
    }

    public async Task<bool> UpdatePersonAsync(PersonDto personDto)
    {
        try
        {
            ValidatePerson(personDto);

            var person = MapToEntity(personDto);

            var result = await _personData.UpdateAsync(person);

            if (!result)
            {
                _logger.LogWarning("No se pudo actualizar la persona con ID {PersonId}", personDto.Id);
                throw new EntityNotFoundException("Person", personDto.Id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la persona con ID {PersonId}", personDto.Id);
            throw new ExternalServiceException("Base de datos", $"Error al actualizar la persona con ID {personDto.Id}", ex);
        }
    }

    public async Task<bool> UpdatePartialPersonAsync(int id, PersonDto updatedFields)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó actualizar una persona con un ID inválido: {PersonId}", id);
            throw new ValidationException("id", "El ID de la persona debe ser mayor a 0");
        }

        try
        {
            var existingPerson = await _personData.GetByIdAsync(id);
            if (existingPerson == null)
            {
                _logger.LogInformation("No se encontró la persona con ID {PersonId} para actualización parcial", id);
                throw new EntityNotFoundException("Person", id);
            }

            if (!string.IsNullOrWhiteSpace(updatedFields.FirstName))
            {
                existingPerson.FirstName = updatedFields.FirstName;
            }
            if (!string.IsNullOrWhiteSpace(updatedFields.LastName))
            {
                existingPerson.LastName = updatedFields.LastName;
            }
            if (!string.IsNullOrWhiteSpace(updatedFields.Email))
            {
                existingPerson.Email = updatedFields.Email;
            }
            if (!string.IsNullOrWhiteSpace(updatedFields.PhoneNumber))
            {
                existingPerson.PhoneNumber = updatedFields.PhoneNumber;
            }
            if (updatedFields.Status != existingPerson.Status)
            {
                existingPerson.Status = updatedFields.Status;
            }

            var result = await _personData.UpdateAsync(existingPerson);

            if (!result)
            {
                _logger.LogWarning("No se pudo actualizar parcialmente la persona con ID {PersonId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente la persona con ID {id}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar parcialmente la persona con ID {PersonId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente la persona con ID {id}", ex);
        }
    }

    public async Task<bool> SoftDeletePersonAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó realizar una eliminación lógica con un ID inválido: {PersonId}", id);
            throw new ValidationException("id", "El ID de la persona debe ser mayor a 0");
        }

        try
        {
            var person = await _personData.GetByIdAsync(id);
            if (person == null)
            {
                _logger.LogInformation("No se encontró la persona con ID {PersonId} para eliminación lógica", id);
                throw new EntityNotFoundException("Person", id);
            }

            person.Status = false; // Marcar como inactiva

            var result = await _personData.UpdateAsync(person);

            if (!result)
            {
                _logger.LogWarning("No se pudo realizar la eliminación lógica de la persona con ID {PersonId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica de la persona con ID {id}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al realizar la eliminación lógica de la persona con ID {PersonId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica de la persona con ID {id}", ex);
        }
    }

    public async Task<bool> DeletePersonAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó eliminar una persona con un ID inválido: {PersonId}", id);
            throw new ValidationException("id", "El ID de la persona debe ser mayor a 0");
        }

        try
        {
            var result = await _personData.DeleteAsync(id);

            if (!result)
            {
                _logger.LogInformation("No se encontró la persona con ID {PersonId} para eliminar", id);
                throw new EntityNotFoundException("Person", id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la persona con ID {PersonId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al eliminar la persona con ID {id}", ex);
        }
    }


    // Método para validar el DTO
    private void ValidatePerson(PersonDto personDto)
    {
        if (personDto == null)
        {
            throw new ValidationException("El objeto persona no puede ser nulo");
        }

        if (string.IsNullOrWhiteSpace(personDto.FirstName))
        {
            _logger.LogWarning("Se intentó crear/actualizar una persona con Name vacío");
            throw new ValidationException("Name", "El Name de la persona es obligatorio");
        }
    }

    // Método para mapear de Person a PersonDto
    private PersonDto MapToDTO(Person person)
    {
        return new PersonDto
        {
            Id = person.Id,
            FirstName = person.FirstName,
            LastName = person.LastName,
            PhoneNumber = person.PhoneNumber,
            Email = person.Email,
            Status = person.Status,
            TypeIdentification = person.TypeIdentification,
            NumberIndification = person.NumberIndification
        };
    }

    // Método para mapear de PersonDto a Person
    private Person MapToEntity(PersonDto personDto)
    {
        return new Person
        {
            Id = personDto.Id,
            FirstName = personDto.FirstName,
            LastName = personDto.LastName,
            PhoneNumber = personDto.PhoneNumber,
            Email = personDto.Email,
            Status = personDto.Status,
            TypeIdentification = personDto.TypeIdentification,
            NumberIndification = personDto.NumberIndification
        };
    }

    // Método para mapear una lista de Person a una lista de PersonDto
    private IEnumerable<PersonDto> MapToDTOList(IEnumerable<Person> persons)
    {
        var personsDTO = new List<PersonDto>();
        foreach (var person in persons)
        {
            personsDTO.Add(MapToDTO(person));
        }
        return personsDTO;
    }
}