// src/app/services/user.service.ts

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../environments/environment';
import { UserResponseDTO } from '../models/dto/user/user-response-dto';
import { GetUserByNameDTO } from '../models/dto/user/get-user-by-name-dto';
import { UpdateUserDTO } from '../models/dto/user/update-user-dto';
import { RegisterUserToCompanyDTO } from '../models/dto/user/register-user-to-company-dto';
import { AddUserToWarehouseDto } from '../models/dto/auth/add-user-to-warehouse-dto';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private readonly baseUrl = environment.baseAuthUrl;
  private readonly apiUrls = environment.apiUrls.user;

  constructor(private http: HttpClient) {}

  getAll(): Observable<UserResponseDTO[]> {
    const url = `${this.baseUrl}${this.apiUrls.getAll}`;
    return this.http.get<UserResponseDTO[]>(url);
  }

  getById(id: string | null): Observable<UserResponseDTO> {
    if (!id) {
      throw new Error('ID пользователя не может быть null');
    }
    const url = `${this.baseUrl}${this.apiUrls.getById.replace('{id}', id)}`;
    return this.http.get<UserResponseDTO>(url);
  }


  getByLogin(login: string): Observable<UserResponseDTO> {
    const url = `${this.baseUrl}${this.apiUrls.getByLogin.replace('{login}', login)}`;
    return this.http.get<UserResponseDTO>(url);
  }

  getByName(dto: GetUserByNameDTO): Observable<UserResponseDTO> {
    const url = `${this.baseUrl}${this.apiUrls.getByName}`;
    return this.http.post<UserResponseDTO>(url, dto);
  }

  getByCompanyId(companyId: string): Observable<UserResponseDTO[]> {
    const url = `${this.baseUrl}${this.apiUrls.getByCompanyId.replace('{companyId}', companyId)}`;
    return this.http.get<UserResponseDTO[]>(url);
  }

  registerUserToCompany(dto: RegisterUserToCompanyDTO): Observable<void> {
    const url = `${this.baseUrl}${this.apiUrls.registerUserToCompany}`;
    return this.http.post<void>(url, dto);
  }

  update(dto: UpdateUserDTO): Observable<void> {
    const url = `${this.baseUrl}${this.apiUrls.update}`;
    return this.http.put<void>(url, dto);
  }

  delete(id: string): Observable<void> {
    const url = `${this.baseUrl}${this.apiUrls.delete.replace('{id}', id)}`;
    return this.http.delete<void>(url);
  }

  addUserToCompany(dto: RegisterUserToCompanyDTO): Observable<void> {
    const url = `${this.baseUrl}${this.apiUrls.addUserToCompany}`;
    return this.http.post<void>(url, dto);
  }

  addUserToWarehouse(dto: AddUserToWarehouseDto): Observable<void> {
    const url = `${environment.baseAuthUrl}${environment.apiUrls.user.addUserToWarehouse}`;
    return this.http.put<void>(url, dto);
  }

}
