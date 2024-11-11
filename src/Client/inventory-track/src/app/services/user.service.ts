import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import {UserResponseDTO} from "../models/dto/user/user-response-dto";
import {GetUserByNameDTO} from "../models/dto/user/get-user-by-name-dto";
import {UpdateUserDTO} from "../models/dto/user/update-user-dto";
import {RegisterUserToCompanyDTO} from "../models/dto/user/register-user-to-company-dto";

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly apiUrl = `${environment.apiUrl}/api/users`;

  constructor(private http: HttpClient) {}

  // Получить всех пользователей
  getAll(): Observable<UserResponseDTO[]> {
    return this.http.get<UserResponseDTO[]>(this.apiUrl);
  }

  // Получить пользователя по ID
  getById(id: string | null): Observable<UserResponseDTO> {
    return this.http.get<UserResponseDTO>(`${this.apiUrl}/${id}`);
  }

  // Получить пользователя по логину
  getByLogin(login: string): Observable<UserResponseDTO> {
    return this.http.get<UserResponseDTO>(`${this.apiUrl}/by-login/${login}`);
  }

  // Получить пользователя по имени
  getByName(getUserByNameDto: GetUserByNameDTO): Observable<UserResponseDTO> {
    return this.http.post<UserResponseDTO>(`${this.apiUrl}/by-name`, getUserByNameDto);
  }

  // Получить пользователей по ID компании
  getByCompanyId(companyId: string): Observable<UserResponseDTO[]> {
    return this.http.get<UserResponseDTO[]>(`${this.apiUrl}/by-company/${companyId}`);
  }

  // Регистрация пользователя в компании
  registerUserToCompany(registerUserToCompanyDto: RegisterUserToCompanyDTO): Observable<void> {
    return this.http.post<void>(this.apiUrl, registerUserToCompanyDto);
  }

  // Обновление пользователя
  update(updateUserDto: UpdateUserDTO): Observable<void> {
    return this.http.put<void>(this.apiUrl, updateUserDto);
  }

  // Удаление пользователя
  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
