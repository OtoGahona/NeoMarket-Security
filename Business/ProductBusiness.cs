using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business;

/// <summary>
/// Clase de negocio encargada de la lógica relacionada con los productos.
/// </summary>
public class ProductBusiness
{
    private readonly ProductData _productData;
    private readonly ILogger<ProductBusiness> _logger;

    public ProductBusiness(ProductData productData, ILogger<ProductBusiness> logger)
    {
        _productData = productData;
        _logger = logger;
    }

    // Método para obtener todos los productos como DTOs
    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        try
        {
            var products = await _productData.GetAllAsync();
            return MapToDTOList(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los productos");
            throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de productos", ex);
        }
    }

    // Método para obtener un producto por ID como DTO
    public async Task<ProductDto> GetProductByIdAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó obtener un producto con ID inválido: {ProductId}", id);
            throw new Utilities.Exceptions.ValidationException("id", "El ID del producto debe ser mayor que cero");
        }

        try
        {
            var product = await _productData.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogInformation("No se encontró ningún producto con ID: {ProductId}", id);
                throw new EntityNotFoundException("Product", id);
            }

            return MapToDTO(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el producto con ID: {ProductId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al recuperar el producto con ID {id}", ex);
        }
    }

    // Método para crear un producto desde un DTO
    public async Task<ProductDto> CreateProductAsync(ProductDto productDto)
    {
        try
        {
            ValidateProduct(productDto);

            var product = MapToEntity(productDto);

            var productCreado = await _productData.CreateAsync(product);
            return MapToDTO(productCreado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear nuevo producto: {ProductName}", productDto?.NameProduct ?? "null");
            throw new ExternalServiceException("Base de datos", "Error al crear el producto", ex);
        }
    }

    /// <summary>
    /// Actualiza un producto existente en la base de datos.
    /// </summary>
    /// <param name="productDto">El DTO del producto con los datos actualizados.</param>
    /// <returns>True si la actualización fue exitosa, False en caso contrario.</returns>
    public async Task<bool> UpdateProductAsync(ProductDto productDto)
    {
        try
        {
            ValidateProduct(productDto);

            var product = MapToEntity(productDto);

            var result = await _productData.UpdateAsync(product);

            if (!result)
            {
                _logger.LogWarning("No se pudo actualizar el producto con ID {ProductId}", productDto.Id);
                throw new EntityNotFoundException("Product", productDto.Id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el producto con ID {ProductId}", productDto.Id);
            throw new ExternalServiceException("Base de datos", $"Error al actualizar el producto con ID {productDto.Id}", ex);
        }
    }

    /// <summary>
    /// Actualiza campos específicos de un producto existente en la base de datos.
    /// </summary>
    /// <param name="id">El ID del producto a actualizar.</param>
    /// <param name="updatedFields">El DTO con los campos actualizados.</param>
    /// <returns>True si la actualización fue exitosa, False en caso contrario.</returns>
    public async Task<bool> UpdatePartialProductAsync(int id, ProductDto updatedFields)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó actualizar un producto con un ID inválido: {ProductId}", id);
            throw new ValidationException("id", "El ID del producto debe ser mayor a 0");
        }

        try
        {
            var existingProduct = await _productData.GetByIdAsync(id);
            if (existingProduct == null)
            {
                _logger.LogInformation("No se encontró el producto con ID {ProductId} para actualización parcial", id);
                throw new EntityNotFoundException("Product", id);
            }

            if (!string.IsNullOrWhiteSpace(updatedFields.NameProduct))
            {
                existingProduct.NameProduct = updatedFields.NameProduct;
            }
            if (!string.IsNullOrWhiteSpace(updatedFields.Description))
            {
                existingProduct.Description = updatedFields.Description;
            }
            if (updatedFields.Price != existingProduct.Price)
            {
                existingProduct.Price = updatedFields.Price;
            }
            if (updatedFields.Status != existingProduct.Status)
            {
                existingProduct.Status = updatedFields.Status;
            }

            var result = await _productData.UpdateAsync(existingProduct);

            if (!result)
            {
                _logger.LogWarning("No se pudo actualizar parcialmente el producto con ID {ProductId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el producto con ID {id}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar parcialmente el producto con ID {ProductId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el producto con ID {id}", ex);
        }
    }

    /// <summary>
    /// Realiza una eliminación lógica de un producto, marcándolo como inactivo.
    /// </summary>
    /// <param name="id">El ID del producto a eliminar lógicamente.</param>
    /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
    public async Task<bool> SoftDeleteProductAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó realizar una eliminación lógica con un ID inválido: {ProductId}", id);
            throw new ValidationException("id", "El ID del producto debe ser mayor a 0");
        }

        try
        {
            var product = await _productData.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogInformation("No se encontró el producto con ID {ProductId} para eliminación lógica", id);
                throw new EntityNotFoundException("Product", id);
            }

            product.Status = false; // Marcar como inactivo

            var result = await _productData.UpdateAsync(product);

            if (!result)
            {
                _logger.LogWarning("No se pudo realizar la eliminación lógica del producto con ID {ProductId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica del producto con ID {id}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al realizar la eliminación lógica del producto con ID {ProductId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica del producto con ID {id}", ex);
        }
    }

    /// <summary>
    /// Elimina un producto de la base de datos por su ID.
    /// </summary>
    /// <param name="id">El ID del producto a eliminar.</param>
    /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
    public async Task<bool> DeleteProductAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó eliminar un producto con un ID inválido: {ProductId}", id);
            throw new ValidationException("id", "El ID del producto debe ser mayor a 0");
        }

        try
        {
            var result = await _productData.DeleteAsync(id);

            if (!result)
            {
                _logger.LogInformation("No se encontró el producto con ID {ProductId} para eliminar", id);
                throw new EntityNotFoundException("Product", id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el producto con ID {ProductId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al eliminar el producto con ID {id}", ex);
        }
    }


    // Método para validar el DTO
    private void ValidateProduct(ProductDto productDto)
    {
        if (productDto == null)
        {
            throw new Utilities.Exceptions.ValidationException("El objeto producto no puede ser nulo");
        }

        if (string.IsNullOrWhiteSpace(productDto.NameProduct))
        {
            _logger.LogWarning("Se intentó crear/actualizar un producto con Name vacío");
            throw new Utilities.Exceptions.ValidationException("Name", "El Name del producto es obligatorio");
        }
    }

    // Método para mapear de Product a ProductDTO
    private ProductDto MapToDTO(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            NameProduct = product.NameProduct,
            Description = product.Description,
            Price = product.Price,
            Status = product.Status,
            IdInventory = product.IdInventory,
            IdCategory = product.IdCategory,
            IdImageItem = product.IdImageItem
        };
    }

    // Método para mapear de ProductDTO a Product
    private Product MapToEntity(ProductDto productDto)
    {
        return new Product
        {
            Id = productDto.Id,
            NameProduct = productDto.NameProduct,
            Description = productDto.Description,
            Price = productDto.Price,
            Status = productDto.Status,
            IdInventory = productDto.IdInventory,
            IdCategory = productDto.IdCategory,
            IdImageItem = productDto.IdImageItem
        };
    }

    // Método para mapear una lista de Product a una lista de ProductDTO
    private IEnumerable<ProductDto> MapToDTOList(IEnumerable<Product> products)
    {
        var productDtos = new List<ProductDto>();
        foreach (var product in products)
        {
            productDtos.Add(MapToDTO(product));
        }
        return productDtos;
    }
}

