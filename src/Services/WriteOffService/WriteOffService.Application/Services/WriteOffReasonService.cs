using AutoMapper;
using WriteOffService.Application.DTOs.Response.WriteOffReason;
using WriteOffService.Application.Interfaces.Services;
using WriteOffService.Domain.Entities;
using WriteOffService.Domain.Interfaces.UnitOfWork;

namespace WriteOffService.Application.Services;

public class WriteOffReasonService : IWriteOffReasonService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public WriteOffReasonService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<WriteOffReasonResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var reasons = (await _unitOfWork.WriteOffReasons.GetAllAsync(cancellationToken)).Where(reason => reason.Reason=="По причине продажи" || reason.Reason == "Истёк срок годности" || reason.Reason=="Поломка");
        return _mapper.Map<IEnumerable<WriteOffReasonResponseDto>>(reasons);
    }

    public async Task<WriteOffReasonResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var reason = await _unitOfWork.WriteOffReasons.GetByIdAsync(id, cancellationToken);
        return _mapper.Map<WriteOffReasonResponseDto>(reason);
    }

    public async Task<WriteOffReasonResponseDto> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var reason = await _unitOfWork.WriteOffReasons.GetByNameAsync(name, cancellationToken);
        return _mapper.Map<WriteOffReasonResponseDto>(reason);
    }

    public async Task CreateAsync(string name, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.WriteOffReasons.CreateAsync(new WriteOffReason() { Reason = name }, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}