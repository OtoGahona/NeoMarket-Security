using Data;
using Entity.DTO;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Business;

/// <summary>
/// Clase de negocio encargada de la lógica relacionada con las compañías del sistema.
/// </summary>
public class CompanyBusiness
{
    private readonly CompanyData _companyData;
    private readonly ILogger <CompanyBusiness> _logger;

    public CompanyBusiness(CompanyData companyData, ILogger<CompanyBusiness> logger)
    {
        _companyData = companyData;
        _logger = logger;
    }

    // Método para obtener todas las compañías como DTOs
    public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync()
    {
        try
        {
            var companies = await _companyData.GetAllAsync();
            return MapToDTOList(companies);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las compañías");
            throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de compañías", ex);
        }
    }

    // Método para obtener una compañía por ID como DTO
    public async Task<CompanyDto> GetCompanyByIdAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó obtener una compañía con ID inválido: {CompanyId}", id);
            throw new Utilities.Exceptions.ValidationException("id", "El ID de la compañía debe ser mayor que cero");
        }

        try
        {
            var company = await _companyData.GetByIdAsync(id);
            if (company == null)
            {
                _logger.LogInformation("No se encontró ninguna compañía con ID: {CompanyId}", id);
                throw new EntityNotFoundException("Company", id);
            }

            return MapToDTO(company);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la compañía con ID: {CompanyId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al recuperar la compañía con ID {id}", ex);
        }
    }

    // Método para crear una compañía desde un DTO
    public async Task<CompanyDto> CreateCompanyAsync(CompanyDto companyDto)
    {
        try
        {
            ValidateCompany(companyDto);

            var company = MapToEntity(companyDto);
            var createdCompany = await _companyData.CreateAsync(company);

            return MapToDTO(createdCompany);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear nueva compañía: {CompanyName}", companyDto?.NameCompany ?? "null");
            throw new ExternalServiceException("Base de datos", "Error al crear la compañía", ex);
        }
    }

    // Método para actualizar completamente una compañía
    public async Task<bool> UpdateCompanyAsync(CompanyDto companyDto)
    {
        try
        {
            ValidateCompany(companyDto);

            var company = MapToEntity(companyDto);
            var result = await _companyData.UpdateAsync(company);

            if (!result)
            {
                _logger.LogWarning("No se pudo actualizar la compañía con ID {CompanyId}", companyDto.Id);
                throw new EntityNotFoundException("Company", companyDto.Id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la compañía con ID {CompanyId}", companyDto.Id);
            throw new ExternalServiceException("Base de datos", $"Error al actualizar la compañía con ID {companyDto.Id}", ex);
        }
    }

    // Método para actualización parcial de una compañía
    public async Task<bool> UpdatePartialCompanyAsync(int id, CompanyDto updatedFields)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó actualizar una compañía con un ID inválido: {CompanyId}", id);
            throw new ValidationException("id", "El ID de la compañía debe ser mayor a 0");
        }

        try
        {
            var existingCompany = await _companyData.GetByIdAsync(id);
            if (existingCompany == null)
            {
                _logger.LogInformation("No se encontró la compañía con ID {CompanyId} para actualización parcial", id);
                throw new EntityNotFoundException("Company", id);
            }

            if (!string.IsNullOrWhiteSpace(updatedFields.NameCompany))
                existingCompany.NameCompany = updatedFields.NameCompany;

            if (!string.IsNullOrWhiteSpace(updatedFields.Description))
                existingCompany.Description = updatedFields.Description;

            if (updatedFields.PhoneCompany != 0)
                existingCompany.PhoneCompany = updatedFields.PhoneCompany;

            if (!string.IsNullOrWhiteSpace(updatedFields.EmailCompany))
                existingCompany.EmailCompany = updatedFields.EmailCompany;

            if (!string.IsNullOrWhiteSpace(updatedFields.Logo))
                existingCompany.Logo = updatedFields.Logo;

            if (!string.IsNullOrWhiteSpace(updatedFields.NitCompany))
                existingCompany.NitCompany = updatedFields.NitCompany;

            if (updatedFields.Status != existingCompany.Status)
                existingCompany.Status = updatedFields.Status;

            var result = await _companyData.UpdateAsync(existingCompany);

            if (!result)
            {
                _logger.LogWarning("No se pudo actualizar parcialmente la compañía con ID {CompanyId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente la compañía con ID {id}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar parcialmente la compañía con ID {CompanyId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente la compañía con ID {id}", ex);
        }
    }

    // Método para realizar Soft Delete (eliminación lógica)
    public async Task<bool> SoftDeleteCompanyAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó realizar una eliminación lógica con un ID inválido: {CompanyId}", id);
            throw new ValidationException("id", "El ID de la compañía debe ser mayor a 0");
        }

        try
        {
            var company = await _companyData.GetByIdAsync(id);
            if (company == null)
            {
                _logger.LogInformation("No se encontró la compañía con ID {CompanyId} para eliminación lógica", id);
                throw new EntityNotFoundException("Company", id);
            }

            company.Status = false;

            var result = await _companyData.UpdateAsync(company);

            if (!result)
            {
                _logger.LogWarning("No se pudo realizar la eliminación lógica de la compañía con ID {CompanyId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica de la compañía con ID {id}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al realizar la eliminación lógica de la compañía con ID {CompanyId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica de la compañía con ID {id}", ex);
        }
    }

    // Método para eliminar físicamente una compañía
    public async Task<bool> DeleteCompanyAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó eliminar una compañía con un ID inválido: {CompanyId}", id);
            throw new ValidationException("id", "El ID de la compañía debe ser mayor a 0");
        }

        try
        {
            var result = await _companyData.DeleteAsync(id);

            if (!result)
            {
                _logger.LogInformation("No se encontró la compañía con ID {CompanyId} para eliminar", id);
                throw new EntityNotFoundException("Company", id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la compañía con ID {CompanyId}", id);
            throw new ExternalServiceException("Base de datos", $"Error al eliminar la compañía con ID {id}", ex);
        }
    }


    // Método para validar el DTO
    private void ValidateCompany(CompanyDto companyDto)
    {
        if (companyDto == null)
        {
            throw new Utilities.Exceptions.ValidationException("El objeto compañía no puede ser nulo");
        }

        if (string.IsNullOrWhiteSpace(companyDto.NameCompany))
        {
            _logger.LogWarning("Se intentó crear/actualizar una compañía con Name vacío");
            throw new Utilities.Exceptions.ValidationException("Name", "El Name de la compañía es obligatorio");
        }
    }

    // Método para mapear de Company a CompanyDto
    private CompanyDto MapToDTO(Company company)
    {
        return new CompanyDto
        {
            Id = company.Id,
            NameCompany = company.NameCompany,
            Description = company.Description,
            Status = company.Status,
            PhoneCompany = company.PhoneCompany,
            Logo = company.Logo,
            EmailCompany = company.EmailCompany,
            NitCompany = company.NitCompany
        };
    }

    // Método para mapear de CompanyDto a Company
    private Company MapToEntity(CompanyDto companyDto)
    {
        return new Company
        {
            Id = companyDto.Id,
            NameCompany = companyDto.NameCompany,
            Description = companyDto.Description,
            Status = companyDto.Status,
            PhoneCompany = companyDto.PhoneCompany,
            Logo = companyDto.Logo,
            EmailCompany = companyDto.EmailCompany,
            NitCompany = companyDto.NitCompany
        };
    }

    // Método para mapear una lista de Company a una lista de CompanyDto
    private IEnumerable<CompanyDto> MapToDTOList(IEnumerable<Company> companies)
    {
        var companiesDTO = new List<CompanyDto>();
        foreach (var company in companies)
        {
            companiesDTO.Add(MapToDTO(company));
        }
        return companiesDTO;
    }
}

