using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business;

/// <summary>
/// Clase de negocio encargada de la lógica relacionada con las compras del sistema.
/// </summary>
public class BuyoutBusiness
{
    private readonly BuyoutData _buyoutData;
    private readonly ILogger <BuyoutBusiness> _logger;

    public BuyoutBusiness(BuyoutData buyoutData, ILogger <BuyoutBusiness> logger)
    {
        _buyoutData = buyoutData;
        _logger = logger;
    }

    // Método para obtener todas las compras como DTOs
    public async Task<IEnumerable<BuyoutDto>> GetAllBuyoutsAsync()
    {
        try
        {
            var buyouts = await _buyoutData.GetAllAsync();
            return MapToDTOList(buyouts);
        }
        catch (Exception ex )
        {
            _logger.LogError(ex, "Error al obtener todas las compras");
            throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de compras", ex);
        }
    }

    // Método para obtener una compra por ID como DTO
    public async Task<BuyoutDto> GetBuyoutByIdAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó obtener una compra con ID inválido: {BuyoutId}", id);
            throw new Utilities.Exceptions.ValidationException("id", "El ID de la compra debe ser mayor que cero");
        }

        try
        {
            var buyout = await _buyoutData.GetByIdAsync(id);
            if (buyout == null)
            {
                _logger.LogInformation("No se encontró ninguna compra con ID: {BuyoutId}", id);
                throw new EntityNotFoundException("Compra", id);
            }

            return MapToDTO(buyout);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la compra con ID: {BuyoutId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al recuperar la compra con ID {id}", ex);
        }
    }

    // Método para crear una compra desde un DTO
    public async Task<BuyoutDto> CreateBuyoutAsync(BuyoutDto buyoutDto)
    {
        try
        {
            
          ValidateBuyout(buyoutDto);

            var buyout = MapToEntity(buyoutDto);

            var buyoutCreado = await _buyoutData.CreateAsync(buyout);
            return MapToDTO(buyoutCreado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear nueva compra: {BuyoutNombre}", buyoutDto?.Quantity);
            throw new ExternalServiceException("Base de datos", "Error al crear la compra", ex);
        }
    }

    // Método para actualizar una compra existente
    public async Task<bool> UpdateBuyoutAsync(BuyoutDto buyoutDto)
    {
        try
        {
            ValidateBuyout(buyoutDto);

            var buyout = MapToEntity(buyoutDto);

            var result = await _buyoutData.UpdateAsync(buyout);

            if (!result)
            {
                _logger.LogWarning("No se pudo actualizar la compra con ID {BuyoutId}", buyoutDto.Id);
                throw new EntityNotFoundException("Compra", buyoutDto.Id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la compra con ID {BuyoutId}", buyoutDto.Id);
            throw new ExternalServiceException("Base de datos", $"Error al actualizar la compra con ID {buyoutDto.Id}", ex);
        }
    }

    // Método para actualizar parcialmente una compra
    public async Task<bool> UpdatePartialBuyoutAsync(int id, BuyoutDto updatedFields)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó actualizar una compra con un ID inválido: {BuyoutId}", id);
            throw new ValidationException("id", "El ID de la compra debe ser mayor a 0");
        }

        try
        {
            var existingBuyout = await _buyoutData.GetByIdAsync(id);
            if (existingBuyout == null)
            {
                _logger.LogInformation("No se encontró la compra con ID {BuyoutId} para actualización parcial", id);
                throw new EntityNotFoundException("Compra", id);
            }

            // Actualizar solo los campos proporcionados
            if (updatedFields.Quantity > 0)
            {
                existingBuyout.Quantity = updatedFields.Quantity;
            }
            if (updatedFields.Date != DateTime.MinValue)
            {
                existingBuyout.Date = updatedFields.Date;
            }

            var result = await _buyoutData.UpdateAsync(existingBuyout);

            if (!result)
            {
                _logger.LogWarning("No se pudo actualizar parcialmente la compra con ID {BuyoutId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente la compra con ID {id}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar parcialmente la compra con ID {BuyoutId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente la compra con ID {id}", ex);
        }
    }

    // Método para realizar una eliminación lógica de una compra
    public async Task<bool> SoftDeleteBuyoutAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó realizar una eliminación lógica con un ID inválido: {BuyoutId}", id);
            throw new ValidationException("id", "El ID de la compra debe ser mayor a 0");
        }

        try
        {
            var buyout = await _buyoutData.GetByIdAsync(id);
            if (buyout == null)
            {
                _logger.LogInformation("No se encontró la compra con ID {BuyoutId} para eliminación lógica", id);
                throw new EntityNotFoundException("Compra", id);
            }

           // buyout.Status = false;

            var result = await _buyoutData.UpdateAsync(buyout);

            if (!result)
            {
                _logger.LogWarning("No se pudo realizar la eliminación lógica de la compra con ID {BuyoutId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica de la compra con ID {id}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al realizar la eliminación lógica de la compra con ID {BuyoutId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica de la compra con ID {id}", ex);
        }
    }

    // Método para eliminar físicamente una compra
    public async Task<bool> DeleteBuyoutAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó eliminar una compra con un ID inválido: {BuyoutId}", id);
            throw new ValidationException("id", "El ID de la compra debe ser mayor a 0");
        }

        try
        {
            var result = await _buyoutData.DeleteAsync(id);

            if (!result)
            {
                _logger.LogInformation("No se encontró la compra con ID {BuyoutId} para eliminar", id);
                throw new EntityNotFoundException("Compra", id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la compra con ID {BuyoutId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al eliminar la compra con ID {id}", ex);
        }
    }


    // Método para validar el DTO
    private void ValidateBuyout(BuyoutDto buyoutDto)
    {
        if (buyoutDto == null)
        {
            throw new Utilities.Exceptions.ValidationException("El objeto compra no puede ser nulo");
        }

        if (buyoutDto.Quantity < 0)
        {
            _logger.LogWarning("Se intentó crear/actualizar una compra con Name vacío");
            throw new Utilities.Exceptions.ValidationException("Name", "El Name de la compra es obligatorio");
        }
    }

    // Método para mapear de Buyout a BuyoutDTO
    private BuyoutDto MapToDTO(Buyout buyout)
    {
        return new BuyoutDto
        {
            Id = buyout.Id,
            Quantity = buyout.Quantity,
            Date = buyout.Date,
            IdUser = buyout.IdUser,
            IdProduct = buyout.IdProduct
        };
    }

    // Método para mapear de BuyoutDTO a Buyout
    private Buyout MapToEntity(BuyoutDto buyoutDto)
    {
        return new Buyout
        {
            Id = buyoutDto.Id,
            Quantity = buyoutDto.Quantity,
            Date = buyoutDto.Date,
            IdUser = buyoutDto.IdUser,
            IdProduct = buyoutDto.IdProduct 
        };
    }

    // Método para mapear una lista de Buyout a una lista de BuyoutDTO
    private IEnumerable<BuyoutDto> MapToDTOList(IEnumerable<Buyout> buyouts)
    {
        var buyoutsDTO = new List<BuyoutDto>();
        foreach (var buyout in buyouts)
        {
            buyoutsDTO.Add(MapToDTO(buyout));
        }
        return buyoutsDTO;
    }
}
