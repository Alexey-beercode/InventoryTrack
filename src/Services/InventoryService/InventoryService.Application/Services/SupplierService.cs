using AutoMapper;
using BookingService.Application.Exceptions;
using InventoryService.Application.DTOs.Request.Supplier;
using InventoryService.Application.DTOs.Response.Supplier;
using InventoryService.Application.Interfaces.Services;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Interfaces.UnitOfWork;

namespace InventoryService.Application.Services;

public class SupplierService : ISupplierService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public SupplierService(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<SupplierResponseDto> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var supplier = await _unitOfWork.Suppliers.GetByNameAsync(name, cancellationToken);

        if (supplier is null)
        {
            throw new EntityNotFoundException($"Supplier with name : {name}");
        }

        return _mapper.Map<SupplierResponseDto>(supplier);
    }

    public async Task<SupplierResponseDto> GetByAccountNumber(string accountNumber, CancellationToken cancellationToken = default)
    {
        var supplier = await _unitOfWork.Suppliers.GetByAccountNumber(accountNumber, cancellationToken);

        if (supplier is null)
        {
            throw new EntityNotFoundException($"Supplier with account number : {accountNumber}");
        }

        return _mapper.Map<SupplierResponseDto>(supplier);
    }

    public async Task<IEnumerable<SupplierResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var suppliers = await _unitOfWork.Suppliers.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<SupplierResponseDto>>(suppliers);
    }

    public async Task<SupplierResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id, cancellationToken);

        if (supplier is null)
        {
            throw new EntityNotFoundException("Supplier", id);
        }

        return _mapper.Map<SupplierResponseDto>(supplier);
    }

    public async Task CreateAsync(CreateSupplierDto createSupplierDto, CancellationToken cancellationToken = default)
    {
        var existSupplier =
            await _unitOfWork.Suppliers.GetByAccountNumber(createSupplierDto.AccountNumber, cancellationToken);

        if (existSupplier is not null)
        {
            throw new AlreadyExistsException("Supplier", "account number", createSupplierDto.AccountNumber);
        }

        var supplier = _mapper.Map<Supplier>(createSupplierDto);
        await _unitOfWork.Suppliers.CreateAsync(supplier, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id, cancellationToken);
        
        if (supplier is null)
        {
            throw new EntityNotFoundException("Supplier", id);
        }

        _unitOfWork.Suppliers.Delete(supplier);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<SupplierResponseDto>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        var suppliers = await _unitOfWork.Suppliers.GetByCompanyIdAsync(companyId, cancellationToken);
        return _mapper.Map<IEnumerable<SupplierResponseDto>>(suppliers);
    }
}