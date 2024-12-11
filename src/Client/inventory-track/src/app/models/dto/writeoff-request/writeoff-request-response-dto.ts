// src/app/models/dto/writeoff-request/writeoff-request-response-dto.ts
import {DocumentInfoResponseDto} from "../document/document-info-response-dto";
import {WriteOffReasonResponseDto} from "../writeoff-reason/writeoff-reason-response-dto";
import {RequestStatusResponseDto} from "./request-status-response-dto";

export interface WriteOffRequestResponseDto {
  id: string;
  itemId: string;
  warehouseId: string;
  quantity: number;
  status: RequestStatusResponseDto;
  requestDate: string;
  approvedByUserId?: string;
  reason: WriteOffReasonResponseDto;
  documents: DocumentInfoResponseDto[];
}
