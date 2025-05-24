export class CreateBatchWriteOffRequestDto {
  batchNumber!: string;
  reasonId?: string;
  anotherReason?: string;
  companyId!: string;
  requestedByUserId!: string;
}
