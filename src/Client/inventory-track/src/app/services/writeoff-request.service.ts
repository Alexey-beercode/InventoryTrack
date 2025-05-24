// src/app/services/writeoff-request.service.ts (ОБНОВЛЕННЫЙ)
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { WriteOffRequestResponseDto } from '../models/dto/writeoff-request/writeoff-request-response-dto';
import { CreateWriteOffRequestDto } from '../models/dto/writeoff-request/create-writeoff-request-dto';
import { CreateBatchWriteOffRequestDto } from '../models/dto/writeoff-request/create-batch-writeoff-request-dto';
import { UpdateWriteOffRequestDto } from '../models/dto/writeoff-request/update-writeoff-request-dto';
import { WriteOffRequestFilterDto } from '../models/dto/writeoff-request/writeoff-request-filter-dto';
import { map } from "rxjs/operators";

@Injectable({
  providedIn: 'root',
})
export class WriteOffRequestService {
  private readonly baseUrl = environment.baseWriteOffUrl;
  private readonly apiUrls = environment.apiUrls.writeOffRequest;

  constructor(private http: HttpClient) {}

  /**
   * Get write-off requests by warehouse ID.
   * @param warehouseId Warehouse ID.
   * @returns Observable with a list of WriteOffRequestResponseDto.
   */
  getByWarehouseId(warehouseId: string): Observable<WriteOffRequestResponseDto[]> {
    return this.http.get<WriteOffRequestResponseDto[]>(
      `${this.baseUrl}${this.apiUrls.getByWarehouseId.replace('{warehouseId}', warehouseId)}`
    );
  }

  /**
   * Get filtered write-off requests.
   * @param filterDto Filter criteria.
   * @returns Observable with a list of WriteOffRequestResponseDto.
   */
  getFiltered(filterDto: WriteOffRequestFilterDto): Observable<WriteOffRequestResponseDto[]> {
    return this.http.get<WriteOffRequestResponseDto[]>(
      `${this.baseUrl}${this.apiUrls.getFiltered}`,
      { params: { ...filterDto } }
    );
  }

  generateDocument(writeOffRequestId: string): Observable<File> {
    return this.http.get(`${this.baseUrl}${this.apiUrls.generateDocument.replace('{id}', writeOffRequestId)}`, {
      responseType: 'blob'
    }).pipe(
      map(blob => new File([blob], `writeoff-document-${writeOffRequestId}.docx`, {
        type: 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'
      }))
    );
  }

  /**
   * Get write-off requests by status.
   * @param status Request status.
   * @returns Observable with a list of WriteOffRequestResponseDto.
   */
  getByStatus(status: string): Observable<WriteOffRequestResponseDto[]> {
    return this.http.get<WriteOffRequestResponseDto[]>(
      `${this.baseUrl}${this.apiUrls.getByStatus.replace('{status}', status)}`
    );
  }

  /**
   * Get write-off request by ID.
   * @param id Request ID.
   * @returns Observable with WriteOffRequestResponseDto.
   */
  getById(id: string): Observable<WriteOffRequestResponseDto> {
    return this.http.get<WriteOffRequestResponseDto>(
      `${this.baseUrl}${this.apiUrls.getById.replace('{id}', id)}`
    );
  }

  /**
   * Create a new write-off request.
   * @param createDto CreateWriteOffRequestDto.
   * @returns Observable indicating completion.
   */
  create(createDto: CreateWriteOffRequestDto): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}${this.apiUrls.create}`, createDto);
  }

// Заменить метод createBatch в WriteOffRequestService

  /**
   * 🆕 Create batch write-off requests for entire batch.
   * @param createDto CreateBatchWriteOffRequestDto.
   * @returns Observable indicating completion.
   */
  createBatch(createDto: CreateBatchWriteOffRequestDto): Observable<any> {
    // 🔥 Отправляем как JSON, а не FormData
    return this.http.post<any>(`${this.baseUrl}${this.apiUrls.createBatch}`, createDto, {
      headers: {
        'Content-Type': 'application/json'
      }
    });
  }

  /**
   * Update a write-off request.
   * @param updateDto UpdateWriteOffRequestDto.
   * @returns Observable indicating completion.
   */
  update(updateDto: UpdateWriteOffRequestDto): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}${this.apiUrls.update}`, updateDto);
  }

  /**
   * Delete a write-off request by ID.
   * @param id Request ID.
   * @returns Observable indicating completion.
   */
  delete(id: string): Observable<void> {
    return this.http.delete<void>(
      `${this.baseUrl}${this.apiUrls.delete.replace('{id}', id)}`
    );
  }

  approve(requestId: string, userId: string, documentId: string): Observable<void> {
    return this.http.put<void>(
      `${this.baseUrl}${this.apiUrls.approve.replace("{id}", requestId)}`,
      { approvedByUserId: userId, documentId: documentId }
    );
  }

  uploadDocuments(requestId: string, formData: FormData): Observable<void> {
    return this.http.post<void>(
      `${this.baseUrl}${this.apiUrls.uploadDocuments.replace("{id}", requestId)}`,
      formData
    );
  }

  reject(id: string, userId: string): Observable<void> {
    const params = { approvedByUserId: userId };
    return this.http.put<void>(
      `${this.baseUrl}${this.apiUrls.reject.replace('{id}', id)}`,
      null,
      { params }
    );
  }

  // 🆕 Вспомогательные методы для работы с партиями

  /**
   * Group write-off requests by batch number
   * @param requests Array of WriteOffRequestResponseDto
   * @returns Map with batch number as key and requests array as value
   */
  groupByBatch(requests: WriteOffRequestResponseDto[]): Map<string, WriteOffRequestResponseDto[]> {
    const batches = new Map<string, WriteOffRequestResponseDto[]>();

    requests.forEach(request => {
      const batchNumber = request.batchNumber || 'Без партии';
      if (!batches.has(batchNumber)) {
        batches.set(batchNumber, []);
      }
      batches.get(batchNumber)!.push(request);
    });

    return batches;
  }

  /**
   * Check if all requests in batch have the same status
   * @param batchRequests Array of requests for one batch
   * @param status Target status to check
   * @returns true if all requests have the target status
   */
  isBatchInStatus(batchRequests: WriteOffRequestResponseDto[], status: string): boolean {
    return batchRequests.every(request => request.status.name === status);
  }

  /**
   * Get batch summary info
   * @param batchRequests Array of requests for one batch
   * @returns Summary object with totals
   */
  getBatchSummary(batchRequests: WriteOffRequestResponseDto[]) {
    return {
      totalQuantity: batchRequests.reduce((sum, req) => sum + req.quantity, 0),
      requestsCount: batchRequests.length,
      warehousesCount: new Set(batchRequests.map(req => req.warehouseId)).size,
      allApproved: this.isBatchInStatus(batchRequests, 'Создано'),
      allRejected: this.isBatchInStatus(batchRequests, 'Отклонено'),
      allPending: this.isBatchInStatus(batchRequests, 'Запрошено')
    };
  }


}
