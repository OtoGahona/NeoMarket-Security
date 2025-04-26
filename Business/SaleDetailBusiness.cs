using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business;

/// <summary>
/// Clase de negocio encargada de la lógica relacionada con los detalles de las selecciones del sistema.
/// </summary>
public class SaleDetailBusiness
{
    private readonly SeleDetailData _seleDetailData;
    private readonly ILogger <SaleDetailBusiness> _logger;

    public SaleDetailBusiness(SeleDetailData seleDetailData, ILogger <SaleDetailBusiness> logger)
    {
        _seleDetailData = seleDetailData;
        _logger = logger;
    }

    // Método para obtener todos los detalles de selecciones como DTOs
    public async Task<IEnumerable<SaleDetailDTO>> GetAllSeleDetailsAsync()
    {
        try
        {
            var seleDetails = await _seleDetailData.GetAllAsync();
            return MapToDTOList(seleDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los detalles de selecciones");
            throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de detalles de selecciones", ex);
        }
    }

    // Método para obtener un detalle de selección por ID como DTO
    public async Task<SaleDetailDTO> GetSeleDetailByIdAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó obtener un detalle de selección con ID inválido: {SeleDetailId}", id);
            throw new Utilities.Exceptions.ValidationException("id", "El ID del detalle de selección debe ser mayor que cero");
        }

        try
        {
            var seleDetail = await _seleDetailData.GetByIdAsync(id);
            if (seleDetail == null)
            {
                _logger.LogInformation("No se encontró ningún detalle de selección con ID: {SeleDetailId}", id);
                throw new EntityNotFoundException("Detalle de Selección", id);
            }

            return MapToDTO(seleDetail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el detalle de selección con ID: {SeleDetailId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al recuperar el detalle de selección con ID {id}", ex);
        }
    }

    // Método para crear un detalle de selección desde un DTO
    public async Task<SaleDetailDTO> CreateSeleDetailAsync(SaleDetailDTO seleDetailDto)
    {
        try
        {
            ValidateSeleDetail(seleDetailDto);

            var seleDetail = MapToEntity(seleDetailDto);

            var seleDetailCreado = await _seleDetailData.CreateAsync(seleDetail);
            return MapToDTO(seleDetailCreado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear nuevo detalle de selección: {SeleDetailNombre}", seleDetailDto?.Quantity);
            throw new ExternalServiceException("Base de datos", "Error al crear el detalle de selección", ex);
        }
    }

    /// <summary>
    /// Actualiza un detalle de venta existente en la base de datos.
    /// </summary>
    /// <param name="saleDetailDto">El DTO del detalle de venta con los datos actualizados.</param>
    /// <returns>True si la actualización fue exitosa, False en caso contrario.</returns>
    public async Task<bool> UpdateSaleDetailAsync(SaleDetailDTO saleDetailDto)
    {
        try
        {
            ValidateSeleDetail(saleDetailDto);

            var saleDetail = MapToEntity(saleDetailDto);

            var result = await _seleDetailData.UpdateAsync(saleDetail);

            if (!result)
            {
                _logger.LogWarning("No se pudo actualizar el detalle de venta con ID {SaleDetailId}", saleDetailDto.Id);
                throw new EntityNotFoundException("Detalle de Venta", saleDetailDto.Id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el detalle de venta con ID {SaleDetailId}", saleDetailDto.Id);
            throw new ExternalServiceException("Base de datos", $"Error al actualizar el detalle de venta con ID {saleDetailDto.Id}", ex);
        }
    }

    /// <summary>
    /// Actualiza campos específicos de un detalle de venta existente en la base de datos.
    /// </summary>
    /// <param name="id">El ID del detalle de venta a actualizar.</param>
    /// <param name="updatedFields">El DTO con los campos actualizados.</param>
    /// <returns>True si la actualización fue exitosa, False en caso contrario.</returns>
    public async Task<bool> UpdatePartialSaleDetailAsync(int id, SaleDetailDTO updatedFields)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó actualizar un detalle de venta con un ID inválido: {SaleDetailId}", id);
            throw new ValidationException("id", "El ID del detalle de venta debe ser mayor a 0");
        }

        try
        {
            var existingSaleDetail = await _seleDetailData.GetByIdAsync(id);
            if (existingSaleDetail == null)
            {
                _logger.LogInformation("No se encontró el detalle de venta con ID {SaleDetailId} para actualización parcial", id);
                throw new EntityNotFoundException("Detalle de Venta", id);
            }

            if (updatedFields.Quantity != existingSaleDetail.Quantity)
            {
                existingSaleDetail.Quantity = updatedFields.Quantity;
            }
            if (updatedFields.Price != existingSaleDetail.Price)
            {
                existingSaleDetail.Price = updatedFields.Price;
            }

            var result = await _seleDetailData.UpdateAsync(existingSaleDetail);

            if (!result)
            {
                _logger.LogWarning("No se pudo actualizar parcialmente el detalle de venta con ID {SaleDetailId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el detalle de venta con ID {id}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar parcialmente el detalle de venta con ID {SaleDetailId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el detalle de venta con ID {id}", ex);
        }
    }

    /// <summary>
    /// Realiza una eliminación lógica de un detalle de venta, marcándolo como inactivo.
    /// </summary>
    /// <param name="id">El ID del detalle de venta a eliminar lógicamente.</param>
    /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
    public async Task<bool> SoftDeleteSaleDetailAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó realizar una eliminación lógica con un ID inválido: {SaleDetailId}", id);
            throw new ValidationException("id", "El ID del detalle de venta debe ser mayor a 0");
        }

        try
        {
            var saleDetail = await _seleDetailData.GetByIdAsync(id);
            if (saleDetail == null)
            {
                _logger.LogInformation("No se encontró el detalle de venta con ID {SaleDetailId} para eliminación lógica", id);
                throw new EntityNotFoundException("Detalle de Venta", id);
            }

            saleDetail.Quantity = 0; // Ejemplo: marcar como eliminado lógicamente

            var result = await _seleDetailData.UpdateAsync(saleDetail);

            if (!result)
            {
                _logger.LogWarning("No se pudo realizar la eliminación lógica del detalle de venta con ID {SaleDetailId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica del detalle de venta con ID {id}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al realizar la eliminación lógica del detalle de venta con ID {SaleDetailId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica del detalle de venta con ID {id}", ex);
        }
    }

    /// <summary>
    /// Elimina un detalle de venta de la base de datos por su ID.
    /// </summary>
    /// <param name="id">El ID del detalle de venta a eliminar.</param>
    /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
    public async Task<bool> DeleteSaleDetailAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó eliminar un detalle de venta con un ID inválido: {SaleDetailId}", id);
            throw new ValidationException("id", "El ID del detalle de venta debe ser mayor a 0");
        }

        try
        {
            var result = await _seleDetailData.DeleteAsync(id);

            if (!result)
            {
                _logger.LogInformation("No se encontró el detalle de venta con ID {SaleDetailId} para eliminar", id);
                throw new EntityNotFoundException("Detalle de Venta", id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el detalle de venta con ID {SaleDetailId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al eliminar el detalle de venta con ID {id}", ex);
        }
    }



    // Método para validar el DTO
    private void ValidateSeleDetail(SaleDetailDTO seleDetailDto)
    {
        if (seleDetailDto == null)
        {
            throw new Utilities.Exceptions.ValidationException("El objeto detalle de selección no puede ser nulo");
        }

        if (seleDetailDto.Quantity < 0)
        {
            _logger.LogWarning("Se intentó crear/actualizar un detalle de selección con Name vacío");
            throw new Utilities.Exceptions.ValidationException("Name", "El Name del detalle de selección es obligatorio");
        }
    }

    // Método para mapear de SeleDetail a SeleDetailDTO
    private SaleDetailDTO MapToDTO(SaleDetail seleDetail)
    {
        return new SaleDetailDTO
        {
            Id = seleDetail.Id,
            Quantity = seleDetail.Quantity,
            Price = seleDetail.Price,
            IdProduct = seleDetail.IdProduct,
            IdSele = seleDetail.IdSele
        };
    }

    // Método para mapear de SeleDetailDTO a SeleDetail
    private SaleDetail MapToEntity(SaleDetailDTO seleDetailDto)
    {
        return new SaleDetail
        {
            Id = seleDetailDto.Id,
            Quantity = seleDetailDto.Quantity,
            Price = seleDetailDto.Price,
            IdProduct = seleDetailDto.IdProduct,
            IdSele = seleDetailDto.IdSele
        };
    }

    // Método para mapear una lista de SeleDetail a una lista de SeleDetailDTO
    private IEnumerable<SaleDetailDTO> MapToDTOList(IEnumerable<SaleDetail> seleDetails)
    {
        var seleDetailsDTO = new List<SaleDetailDTO>();
        foreach (var seleDetail in seleDetails)
        {
            seleDetailsDTO.Add(MapToDTO(seleDetail));
        }
        return seleDetailsDTO;
    }
}
