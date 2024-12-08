using AutoMapper;
using Microsoft.AspNetCore.Http;
using WriteOffService.Application.DTOs.Request.WriteOffRequest;
using WriteOffService.Application.DTOs.Response.WriteOffRequest;
using WriteOffService.Application.Exceptions;
using WriteOffService.Application.Interfaces.Services;
using WriteOffService.Domain.Entities;
using WriteOffService.Domain.Enums;
using WriteOffService.Domain.Interfaces.UnitOfWork;
using WriteOffService.Domain.Models;

namespace WriteOffService.Application.Services;

public class WriteOffRequestService : IWriteOffRequestService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDocumentService _documentService;
    private readonly IWriteOffReasonService _writeOffReasonService;

    public WriteOffRequestService(IMapper mapper, IUnitOfWork unitOfWork, IDocumentService documentService, IWriteOffReasonService writeOffReasonService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _documentService = documentService;
        _writeOffReasonService = writeOffReasonService;
    }

    public async Task<IEnumerable<WriteOffRequestResponseDto>> GetByWarehouseIdAsync(Guid warehouseId, CancellationToken cancellationToken = default)
    {
        var requests = await _unitOfWork.WriteOffRequests.GetByWarehouseIdAsync(warehouseId, cancellationToken);
        return await AttachDocumentsToRequestsAsync(requests, cancellationToken);
    }

    public async Task<IEnumerable<WriteOffRequestResponseDto>> GetFilteredPagedRequestsAsync(WriteOffRequestFilterDto filterDto, CancellationToken cancellationToken = default)
    {
        var filterModel = _mapper.Map<FilterWriteOffrequestModel>(filterDto);
        var filteredRequests = await _unitOfWork.WriteOffRequests.GetFilteredAndPagedAsync(filterModel, cancellationToken);
        return await AttachDocumentsToRequestsAsync(filteredRequests, cancellationToken);
    }

    public async Task<IEnumerable<WriteOffRequestResponseDto>> GetByStatusAsync(RequestStatus status, CancellationToken cancellationToken = default)
    {
        var requests = await _unitOfWork.WriteOffRequests.GetByStatusAsync(status, cancellationToken);
        return await AttachDocumentsToRequestsAsync(requests, cancellationToken);
    }

    public async Task<WriteOffRequestResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var request = await _unitOfWork.WriteOffRequests.GetByIdAsync(id, cancellationToken)
            ?? throw new EntityNotFoundException("WriteOffRequest", id);

        var requestDto = _mapper.Map<WriteOffRequestResponseDto>(request);
        requestDto.Documents = await _documentService.GetByRequestIdAsync(id, cancellationToken);
        return requestDto;
    }

    public async Task DeleteWriteOffRequestAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var request = await _unitOfWork.WriteOffRequests.GetByIdAsync(id, cancellationToken)
            ?? throw new EntityNotFoundException("WriteOffRequest", id);

        _unitOfWork.WriteOffRequests.Delete(request);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task CreateAsync(CreateWriteOffRequestDto createWriteOffRequestDto, CancellationToken cancellationToken = default)
    {
        var newRequest = _mapper.Map<WriteOffRequest>(createWriteOffRequestDto);
        
        await SetWriteOffReasonAsync(createWriteOffRequestDto, newRequest, cancellationToken);
        
        newRequest.ApprovedByUserId = null;
        newRequest.Status = RequestStatus.Requested;
        newRequest.RequestDate = DateTime.UtcNow;
        
        await _unitOfWork.WriteOffRequests.CreateAsync(newRequest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        var savedRequest = await _unitOfWork.WriteOffRequests.GetByDateAndItemIdAsync(newRequest.ItemId, newRequest.RequestDate, cancellationToken);
        
        await AddDocumentsToRequestAsync(savedRequest, createWriteOffRequestDto.Documents, cancellationToken);
    }

    public async Task UpdateAsync(UpdateWriteOffRequestDto updateWriteOffRequestDto, CancellationToken cancellationToken = default)
    {
        var request = await _unitOfWork.WriteOffRequests.GetByIdAsync(updateWriteOffRequestDto.Id, cancellationToken)
            ?? throw new EntityNotFoundException("WriteOffRequest", updateWriteOffRequestDto.Id);
        
        UpdateRequestStatus(request, updateWriteOffRequestDto);

        _unitOfWork.WriteOffRequests.Update(request);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
    

    private async Task<IEnumerable<WriteOffRequestResponseDto>> AttachDocumentsToRequestsAsync(
        IEnumerable<WriteOffRequest> requests, CancellationToken cancellationToken)
    {
        var requestsDtos = _mapper.Map<IEnumerable<WriteOffRequestResponseDto>>(requests);

        foreach (var requestDto in requestsDtos)
        {
            requestDto.Documents = await _documentService.GetByRequestIdAsync(requestDto.Id, cancellationToken);
        }

        return requestsDtos;
    }

    private async Task SetWriteOffReasonAsync(CreateWriteOffRequestDto createDto, WriteOffRequest newRequest, CancellationToken cancellationToken)
    {
        if (createDto.ReasonId != default || createDto.AnotherReason == null)
            return;

        await _writeOffReasonService.CreateAsync(createDto.AnotherReason, cancellationToken);
        var newReason = await _unitOfWork.WriteOffReasons.GetByNameAsync(createDto.AnotherReason, cancellationToken);

        newRequest.ReasonId = newReason.Id;
        newRequest.Reason = newReason;
    }

    private async Task AddDocumentsToRequestAsync(WriteOffRequest request, IEnumerable<IFormFile> documents, CancellationToken cancellationToken)
    {
        foreach (var file in documents)
        {
            var document = await _documentService.CreateDocumentAsync(file, cancellationToken);
            var newDocument = await _unitOfWork.Documents.GetByNameAsync(document.FileName, cancellationToken);

            var writeOffAct = new WriteOffAct
            {
                Document = newDocument,
                WriteOffRequest = request,
                WriteOffRequestId = request.Id,
                DocumentId = newDocument.Id
            };

            await _unitOfWork.WriteOffActs.CreateAsync(writeOffAct, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private void UpdateRequestStatus(WriteOffRequest request, UpdateWriteOffRequestDto updateDto)
    {
        if (updateDto.Status == RequestStatus.Created || updateDto.Status==RequestStatus.Rejected)
        {
            request.ApprovedByUserId = updateDto.ApprovedByUserId;
        }

        if (updateDto.Status != RequestStatus.None)
        {
            request.Status = updateDto.Status;
        }
    }
}