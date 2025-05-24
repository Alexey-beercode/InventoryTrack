import { DocumentInfoResponseDto } from "../document/document-info-response-dto";
import { WriteOffReasonResponseDto } from "../writeoff-reason/writeoff-reason-response-dto";
import { RequestStatusResponseDto } from "./request-status-response-dto";

export interface WriteOffRequestResponseDto {
  id: string;
  itemId: string;
  warehouseId: string;
  quantity: number;
  companyId: string;
  status: RequestStatusResponseDto;
  requestDate: string;
  approvedByUserId?: string;
  reason: WriteOffReasonResponseDto;
  documentId: string;
  requestedByUserId?: string; // 🆕 Кто создал запрос
  batchNumber?: string; // 🆕 Номер партии
}
