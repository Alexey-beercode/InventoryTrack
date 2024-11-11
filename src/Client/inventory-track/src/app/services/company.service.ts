// src/app/services/company.service.ts

import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { CreateCompanyDTO } from '../models/dto/company/create-company-dto';
import { UpdateCompanyDTO } from '../models/dto/company/update-company-dto';
import { CompanyResponseDTO } from '../models/dto/company/company-response-dto';

@Injectable({
  providedIn: 'root',
})
export class CompanyService {
  private apiUrl = `${environment.apiUrl}/api/companies`;

  constructor(private http: HttpClient) {}

  getById(id: string): Observable<CompanyResponseDTO> {
    return this.http.get<CompanyResponseDTO>(`${this.apiUrl}/${id}`);
  }

  create(companyDto: CreateCompanyDTO): Observable<void> {
    return this.http.post<void>(this.apiUrl, companyDto, {
    });
  }

  update(companyDto: UpdateCompanyDTO): Observable<void> {
    return this.http.put<void>(this.apiUrl, companyDto, {
    });
  }

  getAll(): Observable<CompanyResponseDTO[]> {
    return this.http.get<CompanyResponseDTO[]>(this.apiUrl);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getByUserId(userId: string | null): Observable<CompanyResponseDTO> {
    return this.http.get<CompanyResponseDTO>(`${this.apiUrl}/by-user-id/${userId}`);
  }
}
