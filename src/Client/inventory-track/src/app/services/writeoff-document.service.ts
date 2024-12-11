import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { DocumentInfoResponseDto } from '../models/dto/document/document-info-response-dto';

@Injectable({
  providedIn: 'root',
})
export class WriteOffDocumentService {
  private readonly baseUrl = environment.baseWriteOffUrl;
  private readonly apiUrls = environment.apiUrls.document;

  constructor(private http: HttpClient) {}

  /**
   * Upload a document for write-offs.
   * @param file File to be uploaded.
   * @returns Observable with the ID of the uploaded document.
   */
  uploadDocument(file: File): Observable<string> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<string>(`${this.baseUrl}${this.apiUrls.uploadDocument}`, formData);
  }

  /**
   * Retrieve information about a document by its ID.
   * @param id Document ID.
   * @returns Observable with document information.
   */
  getDocumentInfo(id: string): Observable<DocumentInfoResponseDto> {
    return this.http.get<DocumentInfoResponseDto>(
      `${this.baseUrl}${this.apiUrls.getDocumentInfo.replace('{id}', id)}`
    );
  }

  /**
   * Download a document by its ID.
   * @param id Document ID.
   * @returns Observable with the file blob.
   */
  downloadDocument(id: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}${this.apiUrls.downloadDocument.replace('{id}', id)}`, {
      responseType: 'blob',
    });
  }

  /**
   * Delete a document by its ID.
   * @param id Document ID.
   * @returns Observable indicating completion.
   */
  deleteDocument(id: string): Observable<void> {
    return this.http.delete<void>(
      `${this.baseUrl}${this.apiUrls.deleteDocument.replace('{id}', id)}`
    );
  }
}
