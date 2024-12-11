// src/app/services/writeoff-reason.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { WriteOffReasonResponseDto } from '../models/dto/writeoff-reason/writeoff-reason-response-dto';

@Injectable({
  providedIn: 'root',
})
export class WriteOffReasonService {
  private readonly baseUrl = environment.baseWriteOffUrl;
  private readonly apiUrls = environment.apiUrls.writeOffReason;

  constructor(private http: HttpClient) {}

  /**
   * Retrieve all write-off reasons.
   * @returns Observable with an array of WriteOffReasonResponseDto.
   */
  getAll(): Observable<WriteOffReasonResponseDto[]> {
    return this.http.get<WriteOffReasonResponseDto[]>(`${this.baseUrl}${this.apiUrls.getAll}`);
  }

  /**
   * Retrieve a write-off reason by its ID.
   * @param id Reason ID.
   * @returns Observable with WriteOffReasonResponseDto.
   */
  getById(id: string): Observable<WriteOffReasonResponseDto> {
    return this.http.get<WriteOffReasonResponseDto>(
      `${this.baseUrl}${this.apiUrls.getById.replace('{id}', id)}`
    );
  }

  /**
   * Retrieve a write-off reason by its name.
   * @param name Reason name.
   * @returns Observable with WriteOffReasonResponseDto.
   */
  getByName(name: string): Observable<WriteOffReasonResponseDto> {
    return this.http.get<WriteOffReasonResponseDto>(
      `${this.baseUrl}${this.apiUrls.getByName.replace('{name}', name)}`
    );
  }

  /**
   * Create a new write-off reason.
   * @param name Reason name.
   * @returns Observable indicating completion.
   */
  create(name: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}${this.apiUrls.create}`, { name });
  }
}
