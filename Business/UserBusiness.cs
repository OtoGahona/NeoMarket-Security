using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business;

/// <summary>
/// Clase de negocio encargada de la lógica relacionada con los usuarios del sistema.
/// </summary>
public class UserBusiness
{
    private readonly UserData _userData;
    private readonly ILogger <UserBusiness> _logger;

    public UserBusiness(UserData userData, ILogger <UserBusiness> logger)
    {
        _userData = userData;
        _logger = logger;
    }

    // Método para obtener todos los usuarios como DTOs
    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        try
        {
            var users = await _userData.GetAllAsync();
            return MapToDTOList(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los usuarios");
            throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de usuarios", ex);
        }
    }

    // Método para obtener un usuario por ID como DTO
    public async Task<UserDto> GetUserByIdAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó obtener un usuario con ID inválido: {UserId}", id);
            throw new ValidationException("id", "El ID del usuario debe ser mayor que cero");
        }

        try
        {
            var user = await _userData.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogInformation("No se encontró ningún usuario con ID: {UserId}", id);
                throw new EntityNotFoundException("User", id);
            }

            return MapToDTO(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el usuario con ID: {UserId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al recuperar el usuario con ID {id}", ex);
        }
    }

    // Método para crear un usuario desde un DTO
    public async Task<UserDto> CreateUserAsync(UserDto userDto)
    {
        try
        {
            ValidateUser(userDto);

            var user = MapToEntity(userDto);

            var userCreado = await _userData.CreateAsync(user);

            return MapToDTO(userCreado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear nuevo usuario: {UserNombre}", userDto?.UserName ?? "null");
            throw new ExternalServiceException("Base de datos", "Error al crear el usuario", ex);
        }
    }

    /// <summary>
    /// Actualiza un usuario existente en la base de datos.
    /// </summary>
    /// <param name="userDto">El DTO del usuario con los datos actualizados.</param>
    /// <returns>True si la actualización fue exitosa, False en caso contrario.</returns>
    public async Task<bool> UpdateUserAsync(UserDto userDto)
    {
        try
        {
            ValidateUser(userDto);

            var user = MapToEntity(userDto);

            var result = await _userData.UpdateAsync(user);

            if (!result)
            {
                _logger.LogWarning("No se pudo actualizar el usuario con ID {UserId}", userDto.Id);
                throw new EntityNotFoundException("User", userDto.Id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el usuario con ID {UserId}", userDto.Id);
            throw new ExternalServiceException("Base de datos", $"Error al actualizar el usuario con ID {userDto.Id}", ex);
        }
    }

    /// <summary>
    /// Actualiza campos específicos de un usuario existente en la base de datos.
    /// </summary>
    /// <param name="id">El ID del usuario a actualizar.</param>
    /// <param name="updatedFields">El DTO con los campos actualizados.</param>
    /// <returns>True si la actualización fue exitosa, False en caso contrario.</returns>
    public async Task<bool> UpdatePartialUserAsync(int id, UserDto updatedFields)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó actualizar un usuario con un ID inválido: {UserId}", id);
            throw new ValidationException("id", "El ID del usuario debe ser mayor a 0");
        }

        try
        {
            var existingUser = await _userData.GetByIdAsync(id);
            if (existingUser == null)
            {
                _logger.LogInformation("No se encontró el usuario con ID {UserId} para actualización parcial", id);
                throw new EntityNotFoundException("User", id);
            }

            if (!string.IsNullOrWhiteSpace(updatedFields.UserName))
            {
                existingUser.UserName = updatedFields.UserName;
            }
            if (!string.IsNullOrWhiteSpace(updatedFields.Password))
            {
                existingUser.Password = updatedFields.Password;
            }
            if (updatedFields.Status != existingUser.Status)
            {
                existingUser.Status = updatedFields.Status;
            }

            var result = await _userData.UpdateAsync(existingUser);

            if (!result)
            {
                _logger.LogWarning("No se pudo actualizar parcialmente el usuario con ID {UserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el usuario con ID {id}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar parcialmente el usuario con ID {UserId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el usuario con ID {id}", ex);
        }
    }

    /// <summary>
    /// Realiza una eliminación lógica de un usuario, marcándolo como inactivo.
    /// </summary>
    /// <param name="id">El ID del usuario a eliminar lógicamente.</param>
    /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
    public async Task<bool> SoftDeleteUserAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó realizar una eliminación lógica con un ID inválido: {UserId}", id);
            throw new ValidationException("id", "El ID del usuario debe ser mayor a 0");
        }

        try
        {
            var user = await _userData.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogInformation("No se encontró el usuario con ID {UserId} para eliminación lógica", id);
                throw new EntityNotFoundException("User", id);
            }

            user.Status = false; // Marcar como inactivo

            var result = await _userData.UpdateAsync(user);

            if (!result)
            {
                _logger.LogWarning("No se pudo realizar la eliminación lógica del usuario con ID {UserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica del usuario con ID {id}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al realizar la eliminación lógica del usuario con ID {UserId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica del usuario con ID {id}", ex);
        }
    }

    /// <summary>
    /// Elimina un usuario de la base de datos por su ID.
    /// </summary>
    /// <param name="id">El ID del usuario a eliminar.</param>
    /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
    public async Task<bool> DeleteUserAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó eliminar un usuario con un ID inválido: {UserId}", id);
            throw new ValidationException("id", "El ID del usuario debe ser mayor a 0");
        }

        try
        {
            var result = await _userData.DeleteAsync(id);

            if (!result)
            {
                _logger.LogInformation("No se encontró el usuario con ID {UserId} para eliminar", id);
                throw new EntityNotFoundException("User", id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el usuario con ID {UserId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al eliminar el usuario con ID {id}", ex);
        }
    }


    // Método para validar el DTO
    private void ValidateUser(UserDto userDto)
    {
        if (userDto == null)
        {
            throw new ValidationException("El objeto usuario no puede ser nulo");
        }

        if (string.IsNullOrWhiteSpace(userDto.UserName))
        {
            _logger.LogWarning("Se intentó crear/actualizar un usuario con Name vacío");
            throw new ValidationException("Name", "El Name del usuario es obligatorio");
        }
    }

    // Método para mapear de User a UserDto
    private UserDto MapToDTO(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Password = user.Password,
            Status = user.Status,
            IdPerson = user.IdPerson,
        };
    }

    // Método para mapear de UserDto a User
    private User MapToEntity(UserDto userDto)
    {
        return new User
        {
            Id = userDto.Id,
            UserName = userDto.UserName,
            Password = userDto.Password,
            Status = userDto.Status,
            IdPerson = userDto.IdPerson,
        };
    }

    // Método para mapear una lista de User a una lista de UserDto
    private IEnumerable<UserDto> MapToDTOList(IEnumerable<User> users)
    {
        var usersDTO = new List<UserDto>();
        foreach (var user in users)
        {
            usersDTO.Add(MapToDTO(user));
        }
        return usersDTO;
    }
}