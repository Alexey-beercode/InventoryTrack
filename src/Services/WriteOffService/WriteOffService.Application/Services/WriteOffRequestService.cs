﻿using System.Text.Json;
using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WriteOffService.Application.DTOs.Request.WriteOffRequest;
using WriteOffService.Application.DTOs.Response.WriteOffRequest;
using WriteOffService.Application.Exceptions;
using WriteOffService.Application.Interfaces.Clients;
using WriteOffService.Application.Interfaces.Services;
using WriteOffService.Application.Messaging.Producers;
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
    private readonly WriteOffsRequestProducer _writeOffsRequestProducer;
    private readonly ILogger<WriteOffRequestService> _logger;
    private readonly IInventoryHttpClient _inventoryHttpClient;

    public WriteOffRequestService(IMapper mapper, IUnitOfWork unitOfWork, IDocumentService documentService,
        IWriteOffReasonService writeOffReasonService, WriteOffsRequestProducer writeOffsRequestProducer,
        ILogger<WriteOffRequestService> logger, IInventoryHttpClient inventoryHttpClient)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _documentService = documentService;
        _writeOffReasonService = writeOffReasonService;
        _writeOffsRequestProducer = writeOffsRequestProducer;
        _logger = logger;
        _inventoryHttpClient = inventoryHttpClient;
    }

    public async Task<IEnumerable<WriteOffRequestResponseDto>> GetByWarehouseIdAsync(Guid warehouseId,
        CancellationToken cancellationToken = default)
    {
        var requests = await _unitOfWork.WriteOffRequests.GetByWarehouseIdAsync(warehouseId, cancellationToken);
        return _mapper.Map<IEnumerable<WriteOffRequestResponseDto>>(requests);
    }

    public async Task<IEnumerable<WriteOffRequestResponseDto>> GetByCompanyIdAsync(Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var requests = await _unitOfWork.WriteOffRequests.GetByCompanyIdAsync(companyId, cancellationToken);
        return _mapper.Map<IEnumerable<WriteOffRequestResponseDto>>(requests);
    }

    public async Task<IEnumerable<WriteOffRequestResponseDto>> GetFilteredPagedRequestsAsync(
        WriteOffRequestFilterDto filterDto, CancellationToken cancellationToken = default)
    {
        var filterModel = _mapper.Map<FilterWriteOffrequestModel>(filterDto);
        var filteredRequests =
            await _unitOfWork.WriteOffRequests.GetFilteredAndPagedAsync(filterModel, cancellationToken);
        _logger.LogInformation(JsonSerializer.Serialize(filteredRequests));
        return _mapper.Map<IEnumerable<WriteOffRequestResponseDto>>(filteredRequests);
    }

    public async Task<IEnumerable<WriteOffRequestResponseDto>> GetByStatusAsync(RequestStatus status,
        CancellationToken cancellationToken = default)
    {
        var requests = await _unitOfWork.WriteOffRequests.GetByStatusAsync(status, cancellationToken);
        var requestsDtos = _mapper.Map<IEnumerable<WriteOffRequestResponseDto>>(requests);
        return requestsDtos;
    }

    public async Task<IEnumerable<WriteOffRequestResponseDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var requests = await _unitOfWork.WriteOffRequests.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<WriteOffRequestResponseDto>>(requests);
    }

    public async Task<WriteOffRequestResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var request = await _unitOfWork.WriteOffRequests.GetByIdAsync(id, cancellationToken)
                      ?? throw new EntityNotFoundException("WriteOffRequest", id);

        var requestDto = _mapper.Map<WriteOffRequestResponseDto>(request);
        return requestDto;
    }

    public async Task DeleteWriteOffRequestAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var request = await _unitOfWork.WriteOffRequests.GetByIdAsync(id, cancellationToken)
                      ?? throw new EntityNotFoundException("WriteOffRequest", id);

        _unitOfWork.WriteOffRequests.Delete(request);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task CreateAsync(CreateWriteOffRequestDto createDto, CancellationToken cancellationToken = default)
    {
        var newRequest = _mapper.Map<WriteOffRequest>(createDto);

        newRequest.ApprovedByUserId = null;
        newRequest.Status = RequestStatus.Requested;
        newRequest.RequestDate = DateTime.UtcNow;
        if (createDto.AnotherReason != null)
        {
            await _writeOffReasonService.CreateAsync(createDto.AnotherReason, cancellationToken);
            var reason = await _unitOfWork.WriteOffReasons.GetByNameAsync(createDto.AnotherReason, cancellationToken);
            newRequest.Reason = reason;
        }

        await _unitOfWork.WriteOffRequests.CreateAsync(newRequest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ApproveAsync(ApproveWriteOffRequestDto approveDto, CancellationToken cancellationToken = default)
    {
        var request = await _unitOfWork.WriteOffRequests.GetByIdAsync(approveDto.Id, cancellationToken)
                      ?? throw new EntityNotFoundException("WriteOffRequest", approveDto.Id);

        if (request.Status != RequestStatus.Requested)
        {
            throw new InvalidOperationException("Only requests with status 'Requested' can be approved.");
        }

        request.Status = RequestStatus.Created;
        request.ApprovedByUserId = approveDto.ApprovedByUserId;
        request.DocumentId = approveDto.DocumentId;
        _unitOfWork.WriteOffRequests.Update(request);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        var writeOffMessage = new WriteOffInventoryMessage
        {
            ItemId = request.ItemId,
            WarehouseId = request.WarehouseId,
            Quantity = request.Quantity
        };
        await _writeOffsRequestProducer.SendWriteOffMessageAsync(writeOffMessage);
    }

    public async Task RejectAsync(Guid requestId, Guid approvedByUserId, CancellationToken cancellationToken = default)
    {
        var request = await _unitOfWork.WriteOffRequests.GetByIdAsync(requestId, cancellationToken)
                      ?? throw new EntityNotFoundException("WriteOffRequest", requestId);

        if (request.Status != RequestStatus.Requested)
        {
            throw new InvalidOperationException("Only requests with status 'Requested' can be rejected.");
        }

        request.Status = RequestStatus.Rejected;
        request.ApprovedByUserId = approvedByUserId;

        _unitOfWork.WriteOffRequests.Update(request);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(UpdateWriteOffRequestDto updateWriteOffRequestDto,
        CancellationToken cancellationToken = default)
    {
        var request = await _unitOfWork.WriteOffRequests.GetByIdAsync(updateWriteOffRequestDto.Id, cancellationToken)
                      ?? throw new EntityNotFoundException("WriteOffRequest", updateWriteOffRequestDto.Id);

        UpdateRequestStatus(request, updateWriteOffRequestDto);

        _unitOfWork.WriteOffRequests.Update(request);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Если запрос подтвержден, отправляем сообщение в InventoryService
        if (request.Status == RequestStatus.Created)
        {
            var writeOffMessage = new WriteOffInventoryMessage
            {
                ItemId = request.ItemId,
                WarehouseId = request.WarehouseId,
                Quantity = request.Quantity,
            };

            await _writeOffsRequestProducer.SendWriteOffMessageAsync(writeOffMessage);
        }
    }

    /*private async Task<IEnumerable<WriteOffRequestResponseDto>> AttachDocumentsToRequestsAsync(
        IEnumerable<WriteOffRequest> requests, CancellationToken cancellationToken)
    {
        var requestsDtos = _mapper.Map<IEnumerable<WriteOffRequestResponseDto>>(requests);

        foreach (var requestDto in requestsDtos)
        {
            requestDto.Documents = await _documentService.GetByRequestIdAsync(requestDto.Id, cancellationToken);
        }

        return requestsDtos;
    }*/
    public async Task UploadDocumentsAsync(Guid requestId, List<IFormFile> documents,
        CancellationToken cancellationToken = default)
    {
        var request = await _unitOfWork.WriteOffRequests.GetByIdAsync(requestId, cancellationToken)
                      ?? throw new EntityNotFoundException("WriteOffRequest", requestId);

        await AddDocumentsToRequestAsync(request, documents, cancellationToken);

        _unitOfWork.WriteOffRequests.Update(request);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }


    private async Task SetWriteOffReasonAsync(CreateWriteOffRequestDto createDto, WriteOffRequest newRequest,
        CancellationToken cancellationToken)
    {
        if (createDto.ReasonId != default || createDto.AnotherReason == null)
            return;

        await _writeOffReasonService.CreateAsync(createDto.AnotherReason, cancellationToken);
        var newReason = await _unitOfWork.WriteOffReasons.GetByNameAsync(createDto.AnotherReason, cancellationToken);

        newRequest.ReasonId = newReason.Id;
        newRequest.Reason = newReason;
    }

    private async Task AddDocumentsToRequestAsync(WriteOffRequest request, IEnumerable<IFormFile> documents,
        CancellationToken cancellationToken)
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
        if (updateDto.Status == RequestStatus.Created || updateDto.Status == RequestStatus.Rejected)
        {
            request.ApprovedByUserId = updateDto.ApprovedByUserId;
        }

        if (updateDto.Status != RequestStatus.None)
        {
            request.Status = updateDto.Status;
        }
    }

    public async Task CreateBatchWriteOffRequestAsync(CreateBatchWriteOffRequestDto createDto,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("🚀 Создание запросов на списание партии {BatchNumber}", createDto.BatchNumber);

        // Получаем все товары из партии через HTTP запрос к InventoryService
        var batchData = await _inventoryHttpClient.GetItemsByBatchNumberAsync(createDto.BatchNumber, cancellationToken);

        if (batchData?.Items == null || !batchData.Items.Any())
        {
            throw new EntityNotFoundException($"Партия '{createDto.BatchNumber}' не найдена или пуста");
        }

        var createdRequestsCount = 0;
        WriteOffReason reasonEntity = null;

        // Обработка дополнительной причины, если указана (один раз для всей партии)
        if (!string.IsNullOrEmpty(createDto.AnotherReason))
        {
            await _writeOffReasonService.CreateAsync(createDto.AnotherReason, cancellationToken);
            reasonEntity = await _unitOfWork.WriteOffReasons.GetByNameAsync(createDto.AnotherReason, cancellationToken);
        }

        // Создаем запрос на списание для каждого товара в каждом складе
        foreach (var item in batchData.Items)
        {
            foreach (var warehouseDetail in item.WarehouseDetails)
            {
                var newRequest = new WriteOffRequest
                {
                    ItemId = item.Id,
                    WarehouseId = warehouseDetail.WarehouseId,
                    Quantity = warehouseDetail.Quantity,
                    ReasonId = createDto.ReasonId ?? reasonEntity?.Id ?? Guid.Empty,
                    CompanyId = createDto.CompanyId,
                    Status = RequestStatus.Requested,
                    RequestDate = DateTime.UtcNow,
                    BatchNumber = createDto.BatchNumber
                };

                // Устанавливаем связь с причиной, если она была создана
                if (reasonEntity != null)
                {
                    newRequest.ReasonId = reasonEntity.Id;
                    newRequest.Reason = reasonEntity;
                }

                await _unitOfWork.WriteOffRequests.CreateAsync(newRequest, cancellationToken);
                createdRequestsCount++;
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("✅ Создано {Count} запросов на списание для партии {BatchNumber}",
            createdRequestsCount, createDto.BatchNumber);
    }
}
