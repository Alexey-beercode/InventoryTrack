import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../environments/environment';
import { CompanyResponseDTO } from '../models/dto/company/company-response-dto';
import { CreateCompanyDTO } from '../models/dto/company/create-company-dto';
import { UpdateCompanyDTO } from '../models/dto/company/update-company-dto';

@Injectable({
  providedIn: 'root',
})
export class CompanyService {
  private readonly baseUrl = environment.baseAuthUrl;
  private readonly apiUrls = environment.apiUrls.company;

  constructor(private http: HttpClient) {}

  getById(id: string | null): Observable<CompanyResponseDTO> {
    if (!id) {
      throw new Error('ID компании не может быть null');
    }
    const url = `${this.baseUrl}${this.apiUrls.getById.replace('{id}', id)}`;
    return this.http.get<CompanyResponseDTO>(url);
  }

  create(dto: CreateCompanyDTO): Observable<void> {
    const url = `${this.baseUrl}${this.apiUrls.create}`;
    return this.http.post<void>(url, dto);
  }

  update(dto: UpdateCompanyDTO): Observable<void> {
    const url = `${this.baseUrl}${this.apiUrls.update}`;
    return this.http.put<void>(url, dto);
  }

  getAll(): Observable<CompanyResponseDTO[]> {
    const url = `${this.baseUrl}${this.apiUrls.getAll}`;
    return this.http.get<CompanyResponseDTO[]>(url);
  }

  delete(id: string | null): Observable<void> {
    if (!id) {
      throw new Error('ID компании не может быть null');
    }
    const url = `${this.baseUrl}${this.apiUrls.delete.replace('{id}', id)}`;
    return this.http.delete<void>(url);
  }

  getByUserId(userId: string | null): Observable<CompanyResponseDTO> {
    if (!userId) {
      throw new Error('ID пользователя не может быть null');
    }
    const url = `${this.baseUrl}${this.apiUrls.getByUserId.replace('{userId}', userId)}`;
    return this.http.get<CompanyResponseDTO>(url);
  }
}
