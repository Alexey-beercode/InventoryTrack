// src/app/models/dto/writeoff-request/request-status-response-dto.ts
import {RequestStatus} from "./enums/request-status.enum";

export interface RequestStatusResponseDto {
  value: RequestStatus;
  name: string;
}
