import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { CreateMovementRequestDto } from '../models/dto/movement-request/create-movement-request-dto';
import { ChangeStatusDto } from '../models/dto/movement-request/change-status-dto';
import { MovementRequestResponseDto } from '../models/dto/movement-request/movement-request-response-dto';
import {map} from "rxjs/operators";

@Injectable({
  providedIn: 'root',
})
export class MovementRequestService {
  private readonly baseUrl = environment.baseMovementUrl;
  private readonly apiUrls = environment.apiUrls.movementRequest;

  constructor(private http: HttpClient) {}

  create(dto: CreateMovementRequestDto): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}${this.apiUrls.create}`, dto);
  }

  approve(changeStatusDto: ChangeStatusDto): Observable<void> {
    return this.http.post<void>(
      `${this.baseUrl}${this.apiUrls.approve.replace('{id}', changeStatusDto.requestId)}`,
      changeStatusDto
    );
  }

  reject(changeStatusDto: ChangeStatusDto): Observable<void> {
    return this.http.post<void>(
      `${this.baseUrl}${this.apiUrls.reject.replace('{id}', changeStatusDto.requestId)}`,
      changeStatusDto
    );
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(
      `${this.baseUrl}${this.apiUrls.delete.replace('{id}', id)}`
    );
  }

  getById(id: string): Observable<MovementRequestResponseDto> {
    return this.http.get<MovementRequestResponseDto>(
      `${this.baseUrl}${this.apiUrls.getById.replace('{id}', id)}`
    );
  }

  getByStatus(status: string): Observable<MovementRequestResponseDto[]> {
    return this.http.get<MovementRequestResponseDto[]>(
      `${this.baseUrl}${this.apiUrls.getByStatus.replace('{status}', status)}`
    );
  }

  getByWarehouse(warehouseId: string): Observable<MovementRequestResponseDto[]> {
    return this.http.get<MovementRequestResponseDto[]>(
      `${this.baseUrl}${this.apiUrls.getByWarehouse.replace('{warehouseId}', warehouseId)}`
    );
  }

  finalApprove(id: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}${this.apiUrls.finalApprove.replace('{id}', id)}`, {});
  }

  addDocumentToRequest(documentId: string, requestId: string): Observable<void> {
    return this.http.put<void>(
      `${this.baseUrl}${this.apiUrls.addDocumentToRequest
        .replace('{documentId}', documentId)
        .replace('{requestId}', requestId)}`,
      {}
    );
  }
  generateDocument(requestId: string): Observable<File> {
    return this.http.get(`${this.baseUrl}${this.apiUrls.generateDocument.replace('{id}', requestId)}`, {
      responseType: 'blob'
    }).pipe(
      map(blob => new File([blob], `generated-document-${requestId}.xlsx`, { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' }))
    );
  }


}
