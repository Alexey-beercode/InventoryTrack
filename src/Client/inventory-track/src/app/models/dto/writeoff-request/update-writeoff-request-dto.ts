// src/app/models/dto/writeoff-request/update-writeoff-request-dto.ts
import {RequestStatus} from "./enums/request-status.enum";

export interface UpdateWriteOffRequestDto {
  id: string;
  status: RequestStatus;
  approvedByUserId?: string;
}
