
import { mockInventoryItems } from '../movements/movement-list/mock-inventory-items';
import {WarehouseStateResponseDto} from "../../models/dto/warehouse/warehouse-state-response-dto";

export const mockWarehouseState: WarehouseStateResponseDto = {
  id: 'wh-5',
  name: 'Северный склад',
  type: {
    value: 0,
    name: 'Производственный',
  },
  itemsCount: 3,
  location: 'Минск',
  quantity: 33,
  inventoryItems: mockInventoryItems,
};
