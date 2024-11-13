using AutoMapper;
using BookingService.Application.Exceptions;
using MovementService.Application.DTOs.Request.MovementRequest;
using MovementService.Application.DTOs.Response.MovementRequest;
using MovementService.Application.Interfaces.Services;
using MovementService.Domain.Entities;
using MovementService.Domain.Enums;
using MovementService.Domain.Interfaces.UnitOfWork;

namespace MovementService.Application.Services
{
    public class MovementRequestService : IMovementRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MovementRequestService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
    }
}