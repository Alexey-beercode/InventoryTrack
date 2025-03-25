using AutoMapper;
using BookingService.Application.Exceptions;
using Contracts;
using MovementService.Application.DTOs.Request.MovementRequest;
using MovementService.Application.DTOs.Response.MovementRequest;
using MovementService.Application.Interfaces.Services;
using MovementService.Domain.Entities;
using MovementService.Domain.Enums;
using MovementService.Domain.Interfaces.UnitOfWork;
using MovementService.Infrastructure.Messaging.Producers;

namespace MovementService.Application.Services
{
    public class MovementRequestService : IMovementRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MovementRequestProducer _movementRequestProducer;

        public MovementRequestService(IUnitOfWork unitOfWork, IMapper mapper, MovementRequestProducer movementRequestProducer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _movementRequestProducer = movementRequestProducer;
        }

        public async Task CreateMovementRequestAsync(CreateMovementRequestDto dto, CancellationToken cancellationToken = default)
        {
            var movementRequest = _mapper.Map<MovementRequest>(dto);
            movementRequest.Status = MovementRequestStatus.Processing;
            movementRequest.RequestDate = DateTime.UtcNow;

            await _unitOfWork.MovementRequests.CreateAsync(movementRequest, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        

        public async Task RejectMovementRequestAsync(ChangeStatusDto changeStatusDto, CancellationToken cancellationToken = default)
        {
            var movementRequest = await _unitOfWork.MovementRequests.GetByIdAsync(changeStatusDto.RequestId, cancellationToken);
            if (movementRequest == null)
            {
                throw new EntityNotFoundException(nameof(MovementRequest), changeStatusDto.RequestId);
            }

            movementRequest.Status = MovementRequestStatus.Rejected;
            movementRequest.ApprovedByUserId = changeStatusDto.UserId;

            _unitOfWork.MovementRequests.Update(movementRequest);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task CompleteMovementRequestAsync(ChangeStatusDto changeStatusDto, CancellationToken cancellationToken = default)
        {
            var movementRequest = await _unitOfWork.MovementRequests.GetByIdAsync(changeStatusDto.RequestId, cancellationToken);
            if (movementRequest == null)
            {
                throw new EntityNotFoundException(nameof(MovementRequest), changeStatusDto.RequestId);
            }

            movementRequest.Status = MovementRequestStatus.Completed;
            movementRequest.ApprovedByUserId = changeStatusDto.UserId;

            _unitOfWork.MovementRequests.Update(movementRequest);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }


        public async Task DeleteMovementRequestAsync(Guid requestId, CancellationToken cancellationToken = default)
        {
            var movementRequest = await _unitOfWork.MovementRequests.GetByIdAsync(requestId, cancellationToken);
            if (movementRequest == null)
            {
                throw new EntityNotFoundException(nameof(MovementRequest), requestId);
            }

            _unitOfWork.MovementRequests.Delete(movementRequest);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<MovementRequestResponseDto> GetMovementRequestByIdAsync(Guid requestId, CancellationToken cancellationToken = default)
        {
            var movementRequest = await _unitOfWork.MovementRequests.GetByIdAsync(requestId, cancellationToken);
            if (movementRequest == null)
            {
                throw new EntityNotFoundException(nameof(MovementRequest), requestId);
            }

            return _mapper.Map<MovementRequestResponseDto>(movementRequest);
        }

        public async Task<IEnumerable<MovementRequestResponseDto>> GetMovementRequestsByItemIdAsync(Guid itemId, CancellationToken cancellationToken = default)
        {
            var movementRequests = await _unitOfWork.MovementRequests.GetByItemIdAsync(itemId, cancellationToken);
            return _mapper.Map<IEnumerable<MovementRequestResponseDto>>(movementRequests);
        }

        public async Task<IEnumerable<MovementRequestResponseDto>> GetMovementRequestsBySourceWarehouseIdAsync(Guid sourceWarehouseId, CancellationToken cancellationToken = default)
        {
            var movementRequests = await _unitOfWork.MovementRequests.GetBySourceWarehouseIdAsync(sourceWarehouseId, cancellationToken);
            return _mapper.Map<IEnumerable<MovementRequestResponseDto>>(movementRequests);
        }

        public async Task<IEnumerable<MovementRequestResponseDto>> GetMovementRequestsByAnyWarehouseIdAsync(Guid warehouseId, CancellationToken cancellationToken = default)
        { 
            var movementRequests=await _unitOfWork.MovementRequests.GetByAnyWarehouseIdAsync(warehouseId, cancellationToken);
            return _mapper.Map<IEnumerable<MovementRequestResponseDto>>(movementRequests);
        }

        public async Task<IEnumerable<MovementRequestResponseDto>> GetMovementRequestsByDestinationWarehouseIdAsync(Guid destinationWarehouseId, CancellationToken cancellationToken = default)
        {
            var movementRequests = await _unitOfWork.MovementRequests.GetByDestinationWarehouseIdAsync(destinationWarehouseId, cancellationToken);
            return _mapper.Map<IEnumerable<MovementRequestResponseDto>>(movementRequests);
        }

        public async Task<IEnumerable<MovementRequestResponseDto>> GetMovementRequestsByStatusAsync(MovementRequestStatus status, CancellationToken cancellationToken = default)
        {
            var movementRequests = await _unitOfWork.MovementRequests.GetByStatusAsync(status, cancellationToken);
            return _mapper.Map<IEnumerable<MovementRequestResponseDto>>(movementRequests);
        }

        public async Task FinalApproveMovementRequestAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var movementRequest = await _unitOfWork.MovementRequests.GetByIdAsync(id, cancellationToken);
            if (movementRequest is null)
            {
                throw new EntityNotFoundException($"Movement request with id : {id} are not found");
            }

            if (movementRequest.Status == MovementRequestStatus.Final)
            {
                throw new InvalidOperationException("Movement request already completed");
            }
            
            movementRequest.Status = MovementRequestStatus.Final;

            _unitOfWork.MovementRequests.Update(movementRequest);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            var moveMessage = new MoveInventoryMessage
            {
                ItemId = movementRequest.ItemId,
                SourceWarehouseId = movementRequest.SourceWarehouseId,
                DestinationWarehouseId = movementRequest.DestinationWarehouseId,
                Quantity = movementRequest.Quantity
            };

            await _movementRequestProducer.SendMoveInventoryMessageAsync(moveMessage);
        }

        public async Task AddDocumentToMovementRequestAsync(Guid documentId, Guid movementId, CancellationToken cancellationToken = default)
        {
            var movementRequest=await _unitOfWork.MovementRequests.GetByIdAsync(movementId, cancellationToken);
            if (movementRequest is null)
            {
                throw new EntityNotFoundException($"Movement request with id : {movementId} is not found");
            }

            movementRequest.DocumentId = documentId;
            _unitOfWork.MovementRequests.Update(movementRequest);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}