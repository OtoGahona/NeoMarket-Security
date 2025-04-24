using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business;

/// <summary>
/// Clase de negocio encargada de la lógica relacionada con las selecciones del sistema.
/// </summary>
public class SaleBusiness
{
    private readonly SaleData _seleData;
    private readonly ILogger <SaleBusiness> _logger;

    public SaleBusiness(SaleData seleData, ILogger <SaleBusiness> logger)
    {
        _seleData = seleData;
        _logger = logger;
    }

    // Método para obtener todas las selecciones como DTOs
    public async Task<IEnumerable<SaleDTO>> GetAllSeleAsync()
    {
        try
        {
            var selecciones = await _seleData.GetAllAsync();
            return MapToDTOList(selecciones);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las selecciones");
            throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de selecciones", ex);
        }
    }

    // Método para obtener una selección por ID como DTO
    public async Task<SaleDTO> GetSeleByIdAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó obtener una selección con ID inválido: {SeleId}", id);
            throw new Utilities.Exceptions.ValidationException("id", "El ID de la selección debe ser mayor que cero");
        }

        try
        {
            var seleccion = await _seleData.GetByIdAsync(id);
            if (seleccion == null)
            {
                _logger.LogInformation("No se encontró ninguna selección con ID: {SeleId}", id);
                throw new EntityNotFoundException("Selección", id);
            }

            return MapToDTO(seleccion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la selección con ID: {SeleId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al recuperar la selección con ID {id}", ex);
        }
    }

    // Método para crear una selección desde un DTO
    public async Task<SaleDTO> CreateSeleAsync(SaleDTO seleDto)
    {
        try
        {
            ValidateSele(seleDto);

            var seleccion = MapToEntity(seleDto);

            var seleccionCreada = await _seleData.CreateAsync(seleccion);
            return MapToDTO(seleccionCreada);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear nueva selección: {SeleNombre}", seleDto?.Totaly);
            throw new ExternalServiceException("Base de datos", "Error al crear la selección", ex);
        }
    }

    /// <summary>
    /// Actualiza una venta existente en la base de datos.
    /// </summary>
    /// <param name="saleDto">El DTO de la venta con los datos actualizados.</param>
    /// <returns>True si la actualización fue exitosa, False en caso contrario.</returns>
    public async Task<bool> UpdateSaleAsync(SaleDTO saleDto)
    {
        try
        {
            ValidateSele(saleDto);

            var sale = MapToEntity(saleDto);

            var result = await _seleData.UpdateAsync(sale);

            if (!result)
            {
                _logger.LogWarning("No se pudo actualizar la venta con ID {SaleId}", saleDto.Id);
                throw new EntityNotFoundException("Venta", saleDto.Id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la venta con ID {SaleId}", saleDto.Id);
            throw new ExternalServiceException("Base de datos", $"Error al actualizar la venta con ID {saleDto.Id}", ex);
        }
    }

    /// <summary>
    /// Actualiza campos específicos de una venta existente en la base de datos.
    /// </summary>
    /// <param name="id">El ID de la venta a actualizar.</param>
    /// <param name="updatedFields">El DTO con los campos actualizados.</param>
    /// <returns>True si la actualización fue exitosa, False en caso contrario.</returns>
    public async Task<bool> UpdatePartialSaleAsync(int id, SaleDTO updatedFields)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó actualizar una venta con un ID inválido: {SaleId}", id);
            throw new ValidationException("id", "El ID de la venta debe ser mayor a 0");
        }

        try
        {
            var existingSale = await _seleData.GetByIdAsync(id);
            if (existingSale == null)
            {
                _logger.LogInformation("No se encontró la venta con ID {SaleId} para actualización parcial", id);
                throw new EntityNotFoundException("Venta", id);
            }

            if (updatedFields.Totaly != existingSale.Totaly)
            {
                existingSale.Totaly = updatedFields.Totaly;
            }
            if (updatedFields.Date != existingSale.Date)
            {
                existingSale.Date = updatedFields.Date;
            }

            var result = await _seleData.UpdateAsync(existingSale);

            if (!result)
            {
                _logger.LogWarning("No se pudo actualizar parcialmente la venta con ID {SaleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente la venta con ID {id}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar parcialmente la venta con ID {SaleId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente la venta con ID {id}", ex);
        }
    }

    /// <summary>
    /// Realiza una eliminación lógica de una venta, marcándola como inactiva.
    /// </summary>
    /// <param name="id">El ID de la venta a eliminar lógicamente.</param>
    /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
    public async Task<bool> SoftDeleteSaleAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó realizar una eliminación lógica con un ID inválido: {SaleId}", id);
            throw new ValidationException("id", "El ID de la venta debe ser mayor a 0");
        }

        try
        {
            var sale = await _seleData.GetByIdAsync(id);
            if (sale == null)
            {
                _logger.LogInformation("No se encontró la venta con ID {SaleId} para eliminación lógica", id);
                throw new EntityNotFoundException("Venta", id);
            }

            sale.Totaly = 0; // Ejemplo: marcar como eliminada lógicamente

            var result = await _seleData.UpdateAsync(sale);

            if (!result)
            {
                _logger.LogWarning("No se pudo realizar la eliminación lógica de la venta con ID {SaleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica de la venta con ID {id}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al realizar la eliminación lógica de la venta con ID {SaleId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica de la venta con ID {id}", ex);
        }
    }

    /// <summary>
    /// Elimina una venta de la base de datos por su ID.
    /// </summary>
    /// <param name="id">El ID de la venta a eliminar.</param>
    /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
    public async Task<bool> DeleteSaleAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó eliminar una venta con un ID inválido: {SaleId}", id);
            throw new ValidationException("id", "El ID de la venta debe ser mayor a 0");
        }

        try
        {
            var result = await _seleData.DeleteAsync(id);

            if (!result)
            {
                _logger.LogInformation("No se encontró la venta con ID {SaleId} para eliminar", id);
                throw new EntityNotFoundException("Venta", id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la venta con ID {SaleId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al eliminar la venta con ID {id}", ex);
        }
    }



    // Método para validar el DTO
    private void ValidateSele(SaleDTO seleDto)
    {
        if (seleDto == null)
        {
            throw new Utilities.Exceptions.ValidationException("El objeto selección no puede ser nulo");
        }

        if (seleDto.Totaly < 0)
        {
            _logger.LogWarning("Se intentó crear/actualizar una selección con un valor negativo en Totaly: {Totaly}", seleDto.Totaly);
            throw new Utilities.Exceptions.ValidationException("Totaly", "El valor de Totaly no puede ser negativo");
        }

    }

    // Método para mapear de Sele a SeleDTO
    private SaleDTO MapToDTO(Sale seleccion)
    {
        return new SaleDTO
        {
            Id = seleccion.Id,
            Date = seleccion.Date,
            Totaly = seleccion.Totaly,

        };
    }

    // Método para mapear de SeleDTO a Sele
    private Sale MapToEntity(SaleDTO seleDto)
    {
        return new Sale
        {
            Id = seleDto.Id,
            Date = seleDto.Date,
            Totaly = seleDto.Totaly
        };
    }

    // Método para mapear una lista de Sele a una lista de SeleDTO
    private IEnumerable<SaleDTO> MapToDTOList(IEnumerable<Sale> selecciones)
    {
        var seleccionesDTO = new List<SaleDTO>();
        foreach (var seleccion in selecciones)
        {
            seleccionesDTO.Add(MapToDTO(seleccion));
        }
        return seleccionesDTO;
    }
}
