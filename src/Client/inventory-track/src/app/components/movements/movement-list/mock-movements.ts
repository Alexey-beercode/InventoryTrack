import {MovementRequestResponseDto} from '../../../models/dto/movement-request/movement-request-response-dto';
import {MovementRequestStatus} from "../../../models/dto/movement-request/enums/movement-request-status.enum";

export const mockMovements: MovementRequestResponseDto[] = [
  {
    id: 'movement-1',
    itemId: 'item-1',
    sourceWarehouseId: 'wh-1',
    destinationWarehouseId: 'wh-2',
    quantity: 5,
    status: { value: MovementRequestStatus.Pending, name: 'В ожидании' },
    requestDate: new Date('2024-01-10'),
    approvedByUserId: 'user-1',
  },
  {
    id: 'movement-2',
    itemId: 'item-2',
    sourceWarehouseId: 'wh-1',
    destinationWarehouseId: 'wh-3',
    quantity: 10,
    status: { value: MovementRequestStatus.Completed, name: 'Завершено' },
    requestDate: new Date('2024-01-12'),
    approvedByUserId: 'user-2',
  },
  {
    id: 'movement-3',
    itemId: 'item-3',
    sourceWarehouseId: 'wh-4',
    destinationWarehouseId: 'wh-1',
    quantity: 3,
    status: { value: MovementRequestStatus.Rejected, name: 'Отклонено' },
    requestDate: new Date('2024-01-15'),
    approvedByUserId: 'user-3',
  },
];
