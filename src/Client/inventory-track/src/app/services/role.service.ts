import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { RoleDTO } from '../models/dto/role/role-dto';
import { UserRoleDTO } from '../models/dto/user/user-role-dto';

@Injectable({
  providedIn: 'root',
})
export class RoleService {
  private readonly baseUrl = environment.baseAuthUrl;
  private readonly apiUrls = environment.apiUrls.role;

  constructor(private http: HttpClient) {}

  // Получить все роли
  getAll(): Observable<RoleDTO[]> {
    return this.http.get<RoleDTO[]>(`${this.baseUrl}${this.apiUrls.getAll}`);
  }

  // Получить роль по ID
  getById(id: string): Observable<RoleDTO> {
    return this.http.get<RoleDTO>(
      `${this.baseUrl}${this.apiUrls.getById.replace('{id}', id)}`
    );
  }

  // Получить роли пользователя по его ID
  getRolesByUserId(userId: string): Observable<RoleDTO[]> {
    return this.http.get<RoleDTO[]>(
      `${this.baseUrl}${this.apiUrls.getRolesByUserId.replace('{userId}', userId)}`
    );
  }

  // Назначить роль пользователю
  setRoleToUser(userRoleDto: UserRoleDTO): Observable<void> {
    return this.http.post<void>(
      `${this.baseUrl}${this.apiUrls.setRoleToUser}`,
      userRoleDto
    );
  }

  // Удалить роль у пользователя
  removeRoleFromUser(userRoleDto: UserRoleDTO): Observable<void> {
    return this.http.post<void>(
      `${this.baseUrl}${this.apiUrls.removeRoleFromUser}`,
      userRoleDto
    );
  }
}
