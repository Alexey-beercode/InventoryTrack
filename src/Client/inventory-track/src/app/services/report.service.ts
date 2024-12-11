// src/app/services/report.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { ReportResponseDto } from '../models/dto/report/report-response-dto';
import { GetPaginatedReportsQuery } from '../models/dto/report/get-paginated-reports-query';

@Injectable({
  providedIn: 'root',
})
export class ReportService {
  private readonly baseUrl = environment.baseReportUrl;
  private readonly apiUrls = environment.apiUrls.report;

  constructor(private http: HttpClient) {}

  // Получить отчет по ID
  getById(id: string): Observable<Blob> {
    return this.http.get(
      `${this.baseUrl}${this.apiUrls.getById.replace('{id}', id)}`,
      { responseType: 'blob' }
    );
  }

  // Получить отчеты по диапазону дат
  getByDateRange(startDate: string, endDate: string): Observable<ReportResponseDto[]> {
    return this.http.get<ReportResponseDto[]>(
      `${this.baseUrl}${this.apiUrls.getByDateRange
        .replace('{startDate}', startDate)
        .replace('{endDate}', endDate)}`
    );
  }

  // Получить отчеты по выбранной дате
  getByDateSelect(dateSelect: string): Observable<ReportResponseDto[]> {
    return this.http.get<ReportResponseDto[]>(
      `${this.baseUrl}${this.apiUrls.getByDateSelect.replace('{dateSelect}', dateSelect)}`
    );
  }

  // Получить отчеты по типу
  getByType(type: string): Observable<ReportResponseDto[]> {
    return this.http.get<ReportResponseDto[]>(
      `${this.baseUrl}${this.apiUrls.getByType.replace('{type}', type)}`
    );
  }

  // Получить отчеты с пагинацией
  getPaginated(query: GetPaginatedReportsQuery): Observable<ReportResponseDto[]> {
    return this.http.post<ReportResponseDto[]>(
      `${this.baseUrl}${this.apiUrls.getPaginated}`,
      query
    );
  }

  // Получить все отчеты
  getAll(): Observable<ReportResponseDto[]> {
    return this.http.get<ReportResponseDto[]>(`${this.baseUrl}${this.apiUrls.getAll}`);
  }

  // Удалить отчет
  delete(id: string): Observable<void> {
    return this.http.delete<void>(
      `${this.baseUrl}${this.apiUrls.delete.replace('{id}', id)}`
    );
  }
}
