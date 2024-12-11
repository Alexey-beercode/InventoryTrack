// src/app/services/inventory-document.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { DocumentInfoResponseDto } from '../models/dto/document/document-info-response-dto';

@Injectable({
  providedIn: 'root',
})
export class InventoryDocumentService {
  private readonly apiUrl = environment.baseInventoryUrl;

  constructor(private http: HttpClient) {}

  uploadDocument(file: File): Observable<string> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<string>(`${this.apiUrl}${environment.apiUrls.document.uploadDocument}`, formData);
  }

  getDocumentInfo(id: string): Observable<DocumentInfoResponseDto> {
    return this.http.get<DocumentInfoResponseDto>(
      `${this.apiUrl}${environment.apiUrls.document.getDocumentInfo.replace('{id}', id)}`
    );
  }

  downloadDocument(id: string): Observable<Blob> {
    return this.http.get(`${this.apiUrl}${environment.apiUrls.document.downloadDocument.replace('{id}', id)}`, {
      responseType: 'blob',
    });
  }

  deleteDocument(id: string): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}${environment.apiUrls.document.deleteDocument.replace('{id}', id)}`
    );
  }
}
