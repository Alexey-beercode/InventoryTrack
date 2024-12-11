// src/app/models/dto/writeoff-request/writeoff-request-filter-dto.ts
import {RequestStatus} from "./enums/request-status.enum";

export interface WriteOffRequestFilterDto {
  itemId?: string;
  warehouseId?: string;
  quantity?: number;
  reasonId?: string;
  status?: RequestStatus;
  requestDate?: string;
  approvedByUserId?: string;
  pageSize?: number;
  pageNumber?: number;
  companyId?: string;
  isPaginated?: boolean;
  startDate?: string;
  endDate?: string;
}
