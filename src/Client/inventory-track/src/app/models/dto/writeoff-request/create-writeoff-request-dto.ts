// src/app/models/dto/writeoff-request/create-writeoff-request-dto.ts
export interface CreateWriteOffRequestDto {
  itemId: string;
  warehouseId: string;
  quantity: number;
  reasonId?: string | null;
  anotherReason?: string;
  companyId:string;
}
